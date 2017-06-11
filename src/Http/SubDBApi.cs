using SubDBSharp.Http;
using SubDBSharp.Models;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SubDBSharp
{
    public class SubDBApi : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly Uri BaseAddress;
        private readonly HttpClientHandler _httpClientHandler;

        /// <summary>
        /// During development and for tests purposes, please use http://sandbox.thesubdb.com/ as the API url.
        /// </summary>
        /// <param name="productInformation">Product information</param>
        /// <param name="baseAddress">Base address (endpoint)</param>
        public SubDBApi(ProductHeaderValue productInformation, Uri baseAddress)
        {
            BaseAddress = baseAddress;
            _httpClientHandler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            _httpClient = new HttpClient(_httpClientHandler, true);

            // we can't go with this logic because the User-Agent must contain implementor url (information)
            //_httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(productInformation));

            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", Utils.FormatUserAgent(productInformation));
        }

        public Task<Response> GetAvailableLanguagesAsync()
        {
            var uriBuilder = new UriBuilder(BaseAddress) { Query = "action=languages" };
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
            var fullUrl = BaseAddress.ApplySearchSubtitleParameters(hash, getVersions);
            Request request = BuildRequest(fullUrl, HttpMethod.Get, null);
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
            var fullUrl = BaseAddress.ApplyDownloadSubtitleParameters(hash, languages);
            Request request = BuildRequest(fullUrl, HttpMethod.Get, null);
            return SendDataAsync(request);
        }

        /// <summary>
        /// To upload a subtitle, a HTTP POST request will be made to the server.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<Response> UploadSubtitle(string subtitle, string movie)
        {
            var uriBuilder = new UriBuilder(BaseAddress) { Query = "action=upload" };
            Request request = BuildRequest(uriBuilder.Uri, HttpMethod.Post, CreateFormContent(subtitle, movie));
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
            return new HttpRequestMessage()
            {
                RequestUri = request.EndPoint,
                Method = request.Method,
                Content = request.Body
            };
        }

        protected virtual async Task<Response> BuildResponse(HttpResponseMessage responseMessage)
        {
            string responseBody = null;
            using (var content = responseMessage.Content)
            {
                if (content != null)
                {
                    // get can be more processing like getting the metia type before reading
                    // the content information, but subdb always return string
                    responseBody = await content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }
            return new Response(responseMessage.StatusCode, responseBody,
                responseMessage.Headers.ToDictionary(h => h.Key, h => h.Value.First()));
        }

        protected virtual Request BuildRequest(Uri endPoint, HttpMethod method, HttpContent body)
        {
            return new Request(endPoint, method, body);
        }

        private static HttpContent CreateFormContent(string subtitle, string movie)
        {
            const string dispositionType = "form-data";

            var content = new MultipartFormDataContent("xYzZY");

            // hash info
            StringContent stringContent = stringContent = new StringContent(Utils.GetMovieHash(movie));
            stringContent.Headers.ContentDisposition = new ContentDispositionHeaderValue(dispositionType)
            {
                Name = "\"hash\""
            };

            // subtitle file info
            FileStream fs = File.OpenRead(subtitle);
            var streamContent = new StreamContent(fs);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            var dispo = new ContentDispositionHeaderValue(dispositionType)
            {
                Name = "\"file\"",
                FileName = $"\"{Path.GetFileName(subtitle)}\""
            };
            streamContent.Headers.ContentDisposition = dispo;

            // add contents to form
            content.Add(stringContent);
            content.Add(streamContent);

            return content;
        }

        public void Dispose()
        {
            // will dispose _handler aswell (mentioned in contructor)
            _httpClient.Dispose();
            //_httpClientHandler.Dispose();
        }
    }
}