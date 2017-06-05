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

        public async Task<Response> GetLanguagesAvailableAsync()
        {
            var uriBuilder = new UriBuilder(BaseAddress) { Query = "?action=languages" };
            using (HttpResponseMessage responseMessage = await _httpClient.GetAsync(uriBuilder.Uri).ConfigureAwait(false))
            {
                return await BuildResponse(responseMessage);
            }
        }

        /// <summary>
        /// To list the available subtitle languages for a given hash, a HTTP GET request will be made to the server.
        /// </summary>
        /// <param name="hash">The parameter hash is the hash of the video file, generated using our hash function.</param>
        /// <param name="getVersions">This parameter is optional. Using it to return how many versions per language of a subtitle we have in our database.</param>
        /// <returns></returns>
        public async Task<Response> SearchSubtitle(string hash, bool getVersions = false)
        {
            var fullUrl = SubDBApiUrl.ApplySearchSubtitleParameters(hash, getVersions);
            using (HttpResponseMessage resonseMessage = await _httpClient.GetAsync(fullUrl).ConfigureAwait(false))
            {
                //return await resonseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                return await BuildResponse(resonseMessage);
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
                //var responseBody = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                //string fileName = string.Empty;
                //if (responseMessage.Content.Headers.ContentDisposition != null)
                //{
                //    fileName = responseMessage.Content.Headers.ContentDisposition.FileName;
                //}
#if DEBUG
                // according to website "the language of the downloaded subtitle is returned in the
                // content-language field in the header of the response" but it seems to not work :(...
                //if (responseMessage.Content.Headers.ContentLanguage.Any())
                //{
                //    // todo: retrive languge.
                //    // note: this is only usefull if several languages were passed in languages parameter
                //}
#endif
                return await BuildResponse(responseMessage);
            }
        }

        /// <summary>
        /// To upload a subtitle, a HTTP POST request will be made to the server.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Response> UploadSubtitle(string subtitle, string movie)
        {
            const string dispositionType = "form-data";
            using (var content = new MultipartFormDataContent("xYzZY"))
            {
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

                var uriBuilder = new UriBuilder(BaseAddress) { Query = "action=upload" };
                using (HttpResponseMessage response = await _httpClient.PostAsync(uriBuilder.Uri, content))
                {
                    return await BuildResponse(response);
                }
            }
        }

        protected virtual HttpRequestMessage BuildRequestMessage(Request request)
        {
            // all the requests must go through this method
            throw new NotImplementedException();
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

    }
}
