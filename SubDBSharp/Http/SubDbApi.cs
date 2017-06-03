using SubDbSharp.Http;
using SubDbSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SubDbSharp
{
    public class SubDbApi
    {
        public static readonly Uri SubDBApiUrl = new Uri("http://api.thesubdb.com/", UriKind.Absolute);
        private readonly StringBuilder _paramsBuider;
        private readonly HttpClient _httpClient;
        private readonly Uri BaseAddress;
        private readonly HttpClientHandler _httpClientHandler;

        public SubDbApi(ProductHeaderValue productInformation) : this(productInformation, SubDBApiUrl)
        {
        }

        public SubDbApi(ProductHeaderValue productInformation, Uri baseAddress)
        {
            if (baseAddress == null)
            {
                throw new NullReferenceException(nameof(baseAddress));
            }
            // invalid host
            if (baseAddress.Host != SubDBApiUrl.Host)
            {
                throw new InvalidOperationException("Host must be thesubdb");
            }
            BaseAddress = baseAddress;
            _paramsBuider = new StringBuilder();
            _httpClientHandler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            _httpClient = new HttpClient(_httpClientHandler);

            // we can't go with this logic because the User-Agent must contain implementor url (information)
            //_httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(productInformation));

            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", Utils.FormatUserAgent(productInformation));
        }

        public async Task<string> GetLanguagesAvailableAsync()
        {
            var uriBuilder = new UriBuilder(BaseAddress) { Query = "?action=languages" };
            using (HttpResponseMessage response = await _httpClient.GetAsync(uriBuilder.Uri).ConfigureAwait(false))
            {
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// To list the available subtitle languages for a given hash, a HTTP GET request will be made to the server.
        /// </summary>
        /// <param name="hash">The parameter hash is the hash of the video file, generated using our hash function.</param>
        /// <param name="getVersions">This parameter is optional. Using it to return how many versions per language of a subtitle we have in our database.</param>
        /// <returns></returns>
        public async Task<string> SearchSubtitle(string hash, bool getVersions = false)
        {
            var fullUrl = SubDBApiUrl.ApplySearchSubtitleParameters(hash, getVersions);
            using (var response = await _httpClient.GetAsync(fullUrl).ConfigureAwait(false))
            {
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            };
        }

        /// <summary>
        /// To download a subtitle, a HTTP GET request will be made to the server.
        /// </summary>
        /// <param name="hash">The parameter hash is the hash of the video file, generated using our hash function.</param>
        /// <param name="languages"> The parameter language can be a single language code (ex.: us), or a comma separated list in order of priority (ex.: us,nl).
        /// //When using a comma separated list, the first subtitle found is returned.</param>
        /// <returns></returns>
        public async Task<Response> DownloadSubtitle(string hash, params string[] languages)
        {
            var fullUrl = SubDBApiUrl.ApplyDownloadSubtitleParameters(hash, languages);
            using (HttpResponseMessage responseMessage = await _httpClient.GetAsync(fullUrl, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
            {
                var responseBody = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                string file = string.Empty;
                if (responseMessage.Content.Headers.ContentDisposition != null)
                {
                    file = responseMessage.Content.Headers.ContentDisposition.FileName;
                }
                return new Response(
                    responseMessage.StatusCode,
                    responseBody,
                    responseMessage.Headers.ToDictionary(h => h.Key, h => h.Value.First()),
                    file);
            }
        }

        /// <summary>
        /// To upload a subtitle, a HTTP POST request will be made to the server.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> UploadSubtitle(string file)
        {
            const string dispositionType = "form-data";

            using (var content = new MultipartFormDataContent("xYzZY"))
            {
                // hash info
                StringContent stringContent = stringContent = new StringContent(Utils.GetHashString(file));
                stringContent.Headers.ContentDisposition = new ContentDispositionHeaderValue(dispositionType)
                {
                    Name = "\"hash\""
                };

                // subtitle file info
                FileStream fs = File.OpenRead(file);
                var streamContent = new StreamContent(fs);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var dispo = new ContentDispositionHeaderValue(dispositionType)
                {
                    Name = "\"file\"",
                    FileName = $"\"{Path.GetFileName(file)}\""
                };
                streamContent.Headers.ContentDisposition = dispo;

                content.Add(stringContent);
                content.Add(streamContent);

                Uri fullUrl = new Uri("http://sandbox.thesubdb.com/?action=upload");
                Console.WriteLine(_httpClient.DefaultRequestHeaders.UserAgent);
                HttpResponseMessage response = await _httpClient.PostAsync(fullUrl, content);

                return response.StatusCode == HttpStatusCode.Created;
            }
        }

        protected virtual HttpRequestMessage BuildRequestMessage(Request request)
        {
            // all the requests must go through this method
            throw new NotImplementedException();
        }

        protected virtual async Task<Response> BuildResponse(HttpResponseMessage responseMessage)
        {
            object responseBody = null;
            using (var content = responseMessage.Content)
            {
                if (content != null)
                {

                }
            }
            return null;
        }

    }
}
