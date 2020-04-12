using SubDBSharp.Helpers;
using SubDBSharp.Http;
using SubDBSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SubDBSharp
{
    public class SubDBApi : IDisposable, ISubDBApi
    {
        private readonly HttpClient _httpClient;
        private readonly Uri _baseAddress;

        /// <summary>
        /// During development and for tests purposes, please use http://sandbox.thesubdb.com/ as the API url.
        /// </summary>
        /// <param name="productInformation">Product information</param>
        /// <param name="baseAddress">Base address (endpoint)</param>
        public SubDBApi(ProductHeaderValue productInformation, Uri baseAddress)
        {
            _baseAddress = baseAddress;

            // http response message handler
            var httpClientHandler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                AllowAutoRedirect = false
            };

            _httpClient = new HttpClient(httpClientHandler, true)
            {
                BaseAddress = baseAddress,
            };

            // we can't go with this logic because the User-Agent must contain implementor url (information)
            // which doesn't follow standard user-agent format.
            //_httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(productInformation));

            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", Utils.FormatUserAgent(productInformation));
            // _httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
        }

        public Task<Response> GetAvailableLanguagesAsync()
        {
            UriBuilder uriBuilder = new UriBuilder(_baseAddress) { Query = "action=languages" };
            Request request = BuildRequest(uriBuilder.Uri, HttpMethod.Get, null);
            return SendDataAsync(request);
        }

        /// <summary>
        /// To list the available subtitle languages for a given hash, a HTTP GET request will be made to the server.
        /// </summary>
        /// <param name="hash">The parameter hash is the hash of the video file, generated using our hash function.</param>
        /// <param name="getVersions">This parameter is optional. Using it to return how many versions per language of a subtitle we have in our database.</param>
        /// <returns></returns>
        public Task<Response> SearchSubtitle(string hash, bool getVersions = false)
        {

            Uri fullUrl = _baseAddress.ApplySearchSubtitleParameters(hash, getVersions);
            Request request = BuildRequest(fullUrl, HttpMethod.Get, null);
            // NOTE: THIS CODE IS ONLY FOR TEST. (SUBDB DOESN'T ENCODED QUERY STRING)
            // ONCE THE QUERY STRING IS ENCODED IT'S PUT INSIDE HTTP BODY INSTEAD OF URL
            //Dictionary<string, string> requestParameters = new Dictionary<string, string>
            //{
            //    ["action"] = "download",
            //    ["hash"] = hash,
            //    // TODO: Language.
            //};
            //Request request = BuildRequest(new Uri("http://sandbox.thesubdb.com"), HttpMethod.Get, UriExtensions.GetFormURlEncodedContent(requestParameters));

            return SendDataAsync(request);
        }

        /// <summary>
        /// To download a subtitle, a HTTP GET request will be made to the server.
        /// </summary>
        /// <param name="hash">The parameter hash is the hash of the video file, generated using our hash function.</param>
        /// <param name="languages"> The parameter language can be a single language code (ex.: us), or a comma separated list in order of priority (ex.: us,nl).
        /// //When using a comma separated list, the first subtitle found is returned.</param>
        /// <returns></returns>
        public Task<Response> DownloadSubtitle(string hash, params string[] languages)
        {
            Uri fullUrl = _baseAddress.ApplyDownloadSubtitleParameters(hash, languages);
            Request request = BuildRequest(fullUrl, HttpMethod.Get, null);
            return SendDataAsync(request);
        }

        /// <summary>
        /// To upload a subtitle, a HTTP POST request will be made to the server.
        /// </summary>
        public Task<Response> UploadSubtitle(string subtitle, string movie)
        {
            UriBuilder uriBuilder = new UriBuilder(_baseAddress) { Query = "action=upload" };
            Request request = BuildRequest(uriBuilder.Uri, HttpMethod.Post, BuildFormContent(subtitle, movie));
            return SendDataAsync(request);
        }

        // ALL THE REQUEST MUST TO THROUGH THIS METHOD!!!
        private async Task<Response> SendDataAsync(Request request)
        {
            using (HttpRequestMessage requestMessage = BuildRequestMessage(request))
            using (HttpResponseMessage responseMessage = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
            {
                return await BuildResponse(responseMessage);
            }
        }

        protected virtual HttpRequestMessage BuildRequestMessage(Request request)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage()
            {
                RequestUri = request.EndPoint,
                Method = request.Method,
                Content = request.Body,
            };

            // requestMessage.Headers.TryGetValues("Connection", out ...)

            // Note: Will be added twice if this line is uncommented
            // requestMessage.Headers.Connection.Add("keep-alive");

            // HttpRequestHeader is only available in .net45
            //requestMessage.Headers.Remove(HttpRequestHeader.Connection);
            //requestMessage.Headers.Add(HttpRequestHeader.Connection, "keep-alive");

            //requestMessage.Headers.ConnectionClose = false;
            requestMessage.Headers.AcceptCharset.ParseAdd("utf-8");
            return requestMessage;
        }

        protected virtual async Task<Response> BuildResponse(HttpResponseMessage responseMessage)
        {
            object responseBody;
            using (HttpContent content = responseMessage.Content)
            {
                // get can be more processing like getting the media type before reading
                // the content information, but subdb always return string
                responseBody = await content?.ReadAsByteArrayAsync();
            }
            return new Response(responseMessage.StatusCode, responseBody,
                responseMessage.Headers.ToDictionary(h => h.Key, h => h.Value.First()));
        }

        protected virtual Request BuildRequest(Uri endPoint, HttpMethod method, HttpContent body) => new Request
        {
            EndPoint = endPoint,
            Body = body,
            Method = method,
        };

        private static HttpContent BuildFormContent(string subtitle, string movie)
        {
            MultipartFormDataContent content = new MultipartFormDataContent("xYzZY");

            // hash content
            StringContent stringContent = new StringContent(Utils.GetMovieHash(movie));
            stringContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                DispositionType = "form-data",
                Name = "\"hash\""
            };

            // subtitle content
            StreamContent streamContent = new StreamContent(File.OpenRead(subtitle));
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"file\"",
                FileName = $"\"{Path.GetFileName(subtitle)}\""
            };

            // add contents to form
            content.Add(stringContent);
            content.Add(streamContent);

            return content;
        }

        public void Dispose() => _httpClient.Dispose(); // calling dispose in _httClient handler will also dispose _httpClientHandler. (set in contructor)
    }
}