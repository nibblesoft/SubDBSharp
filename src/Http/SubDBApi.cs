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
    public class SubDBApi
    {
        public static readonly Uri SubDBApiUrl = new Uri("http://api.thesubdb.com/", UriKind.Absolute);
        private readonly HttpClient _httpClient;
        private readonly Uri BaseAddress;
        private readonly HttpClientHandler _httpClientHandler;

        public SubDBApi(ProductHeaderValue productInformation) : this(productInformation, SubDBApiUrl)
        {
        }

        public SubDBApi(ProductHeaderValue productInformation, Uri baseAddress)
        {
            if (baseAddress == null)
            {
                throw new NullReferenceException(nameof(baseAddress));
            }
#if !DEBUG
            // invalid host
            if (!(baseAddress.Host == SubDBApiUrl.Host))
            {
                throw new InvalidOperationException("Host must be thesubdb");
            }
#endif
            BaseAddress = baseAddress;
            _httpClientHandler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            _httpClient = new HttpClient(_httpClientHandler);

            // we can't go with this logic because the User-Agent must contain implementor url (information)
            //_httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(productInformation));

            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", Utils.FormatUserAgent(productInformation));
        }

        public Task<Response> GetAvailableLanguagesAsync()
        {
            var uriBuilder = new UriBuilder(BaseAddress) { Query = "action=languages" };
            using (HttpRequestMessage requestMessage = BuildRequestMessage(uriBuilder.Uri, HttpMethod.Get, null))
            {
                return SendDataAsync(requestMessage);
            }
        }

        /// <summary>
        /// To list the available subtitle languages for a given hash, a HTTP GET request will be made to the server.
        /// </summary>
        /// <param name="hash">The parameter hash is the hash of the video file, generated using our hash function.</param>
        /// <param name="getVersions">This parameter is optional. Using it to return how many versions per language of a subtitle we have in our database.</param>
        /// <returns></returns>
        public Task<Response> SearchSubtitle(string hash, bool getVersions = false)
        {
            var fullUrl = SubDBApiUrl.ApplySearchSubtitleParameters(hash, getVersions);
            using (HttpRequestMessage requestMessage = BuildRequestMessage(fullUrl, HttpMethod.Get, null))
            {
                return SendDataAsync(requestMessage);
            }
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
            var fullUrl = SubDBApiUrl.ApplyDownloadSubtitleParameters(hash, languages);
            using (var requestMessage = BuildRequestMessage(fullUrl, HttpMethod.Get, null))
            {
                return SendDataAsync(requestMessage);
            }
        }

        /// <summary>
        /// To upload a subtitle, a HTTP POST request will be made to the server.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<Response> UploadSubtitle(string subtitle, string movie)
        {
            var uriBuilder = new UriBuilder(BaseAddress) { Query = "action=upload" };
            using (HttpContent httpContent = CreateFormContent(subtitle, movie))
            using (HttpRequestMessage httpRequestMessage = BuildRequestMessage(uriBuilder.Uri, HttpMethod.Post, httpContent))
            {
                return SendDataAsync(httpRequestMessage);
            }
        }

        // ALL THE METHOD MUST GO THROUGH THIS!!!
        private async Task<Response> SendDataAsync(HttpRequestMessage requestMessage)
        {
            using (HttpResponseMessage responseMessage = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
            {
                return await BuildResponse(responseMessage);
            }
        }

        protected virtual HttpRequestMessage BuildRequestMessage(Uri endPoint, HttpMethod httpMethod, HttpContent content)
        {
            return new HttpRequestMessage()
            {
                RequestUri = endPoint,
                Method = httpMethod,
                Content = content
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
    }
}