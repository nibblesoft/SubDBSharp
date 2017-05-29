using SubDbSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SubDBSharp
{
    public class SubDBClient
    {
        public static readonly Uri SubDBApiUrl = new Uri("http://api.thesubdb.com/", UriKind.Absolute);
        private readonly StringBuilder _paramsBuider;
        private readonly HttpClient _httpClient;
        private readonly Uri BaseAddress;
        private readonly HttpClientHandler _httpClientHandler;

        #region Constuctors

        public SubDBClient() : this(new ProductHeaderValue("SubDBSharp", "1.1"), SubDBApiUrl)
        {
        }

        public SubDBClient(Uri baseAddress) : this(new ProductHeaderValue("SubDBSharp", "1.1"), baseAddress)
        {
        }

        public SubDBClient(ProductHeaderValue productInformation, Uri baseAddress)
        {
            BaseAddress = baseAddress;
            _paramsBuider = new StringBuilder();
            _httpClientHandler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            _httpClient = new HttpClient(_httpClientHandler);
            //string userAgent = Utils.FormatUserAgent(productInformation);
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(productInformation));
        }

        #endregion

        public async Task<IReadOnlyList<Language>> LanguagesAvailable()
        {
            // TODO: Move this to a helper classe.
            string endPoint = "?action=languages";
            var fullUrl = new Uri(BaseAddress, endPoint);
            //var request = new HttpRequestMessage();
            using (HttpResponseMessage response = await _httpClient.GetAsync(fullUrl).ConfigureAwait(false))
            {
                string responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var readonlyList = new List<Language>();
                foreach (var isoTwoLetter in responseString.Split(','))
                {
                    var lang = new Language(isoTwoLetter);
                    readonlyList.Add(lang);
                }
                return readonlyList;
            }
        }

        public async Task<IReadOnlyList<Language>> SearchSubtitle(string hash, bool getVersions = false)
        {
            string action = $"?action=search&hash={hash}";
            if (getVersions)
                action += $"&versions";
            var fullUrl = new Uri(SubDBApiUrl, action);
            var response = await _httpClient.GetAsync(fullUrl).ConfigureAwait(false);
            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var listLanguages = new List<Language>();
            foreach (var token in responseString.Split(','))
            {
                // Tokens are available only if getVersion == true
                string[] tokens = token.Split(':');
                Language lang = null;
                if (getVersions)
                {
                    lang = new Language(tokens[0], int.Parse(tokens[1]));
                }
                else
                {
                    lang = new Language(tokens[0]);
                }
                if (lang != null)
                    listLanguages.Add(lang);
            }
            return listLanguages;
        }

        public async Task<Response> DownloadSubtitle(string hash, params Language[] languages)
        {
            string endPoint = $"?action=download&hash={hash}" + "&language={0}";
            _paramsBuider.Length = 0;

            // {language.Culture.TwoLetterISOLanguageName}";
            foreach (Language lang in languages)
            {
                _paramsBuider.AppendFormat("{0},", lang.Name);
            }

            // Remove last ','
            _paramsBuider.Remove(_paramsBuider.Length - 1, 1);

            string uriS = string.Format(endPoint, _paramsBuider.ToString());
            Uri endPointUri = new Uri(uriS, UriKind.Relative);
            var fullUrl = new Uri(SubDBApiUrl, endPointUri);

            HttpResponseMessage responseMessage = await _httpClient.GetAsync(fullUrl).ConfigureAwait(false);

            if (responseMessage.Content is ByteArrayContent)
            {

            }
            if (responseMessage.Content is StringContent)
            {

            }
            if (responseMessage.Content is StreamContent)
            {

            }
            Stream responseStreamContent = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
            StreamContent streamContent = new StreamContent(responseStreamContent);
            string file = string.Empty;
            if (responseMessage.Content.Headers.ContentDisposition != null)
            {
                file = responseMessage.Content.Headers.ContentDisposition.FileName;
            }
            return new Response(file, streamContent);
        }

        // TODO: Make a request model for these parameters.
        public async Task<bool> UploadSubtitle(Request request)
        {
            string endPoint = "?action=upload&hash={0}";
            //POST /?action=upload HTTP/1.1
            //Host: api.thesubdb.com
            //User-Agent: SubDB/1.0 (Pyrrot/0.1; http://github.com/jrhames/pyrrot-cli)
            //Content-Length: 60047
            //Content-Type: multipart/form-data; boundary=xYzZY

            //- - --xYzZY
            //Content-Disposition: form-data; name="hash"

            //edc1981d6459c6111fe36205b4aff6c2
            //- - --xYzZY
            //Content-Disposition: form-data; name="file"; filename="subtitle.srt"
            //Content-Type: application/octet-stream


            //[PAYLOAD]
            using (var requestMessage = Utils.BuildRequestMessage(request, endPoint, BaseAddress))
            {
                var response = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
                return response.IsSuccessStatusCode;
            }
        }

    }
}