using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;

namespace SubDBSharp
{
    public class SubDBClient
    {
        // TODO: Dispose.
        private readonly HttpClient _httpClient = new HttpClient();

        public const string ProtocolName = "SubDB";
        public const string ProtocolVersion = "1.0";
        public static readonly Uri SubDBApiUrl = new Uri("http://api.thesubdb.com/", UriKind.Absolute);
        public readonly string UserAgent;
        Client _client;
        public SubDBClient(Client client)
            : this(client, SubDBApiUrl)
        {
        }

        public SubDBClient(Client client, Uri baseAddress)
        {
            _client = client;
            UserAgent = string.Format("{0}/{1} {2}", ProtocolName, ProtocolVersion, client);
            _httpClient.BaseAddress = SubDBApiUrl;
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("SubDB", "1.0"));
        }

        public async Task<IReadOnlyList<Language>> LanguagesAvailable()
        {
            // TODO: Move this to a helper classe.
            string endPoint = "?action=languages";
            Uri fullUrl = new Uri(SubDBApiUrl, endPoint);
            //var request = new HttpRequestMessage();

            HttpResponseMessage response = await _httpClient.GetAsync(fullUrl);
            string responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var readonlyList = new List<Language>();
            foreach (var isoTwoLetter in responseString.Split(','))
            {
                var lang = new Language(isoTwoLetter);
                readonlyList.Add(lang);
            }
            return readonlyList;
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

        public async Task<Stream> DownloadSubtitle(string hash, Language language)
        {
            string endPoints = $"?action=download&hash={hash}&language={language.Culture.TwoLetterISOLanguageName}";
            var fullUrl = new Uri(SubDBApiUrl, endPoints);
            return await _httpClient.GetStreamAsync(fullUrl/*, HttpCompletionOption.ResponseContentRead*/).ConfigureAwait(false);
        }


        public async Task<bool> UploadSubtitle(string movieFile, string subfile)
        {
            string movieHash = Utils.GetHashString(movieFile);
            //string contentBody = GetFormatedBody(movieHash, File.ReadAllText(subfile, Encoding.UTF8));
            //byte[] textBytes = Encoding.UTF8.GetBytes(contentBody);
            string endPoint = $"?action=upload&hash={movieHash}";
            var fullUrl = new Uri(SubDBApiUrl, endPoint);
            //HttpWebRequest request = RunRequestGet(action, WebRequestMethods.Http.Post);
            //request.ContentType = "multipart/form-data; boundary=xYzZY";
            //request.Headers.Add(HttpRequestHeader.Pragma, "no-cache");
            //request.Timeout = 1000 * 10;
            //request.ContentLength = textBytes.Length;
            //Stream requestStream = request.GetRequestStream();
            //requestStream.Write(textBytes, 0, textBytes.Length);

            //try
            //{
            //    WebResponse responseStream = request.GetResponse();
            //}
            //finally
            //{
            //    if (requestStream != null)
            //    {
            //        requestStream.Close();
            //        requestStream.Dispose();
            //    }
            //}

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


            // Subtilte content.
            StringContent stringContent = new StringContent(File.ReadAllText(subfile), Encoding.UTF8);
            StringContent hashString = new StringContent(movieHash);


            // TODO: Prevent adding boundary everytime we try to add a HttpContent. (MultipartContent or MultipartFormDataContent).

            var multiPartContent1 = new MultipartContent("form-data", "xYzZY"); // multipart/form-data?
            multiPartContent1.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
            multiPartContent1.Headers.ContentDisposition.Name = "hash";
            multiPartContent1.Add(hashString);

            var multiPartContent2 = new MultipartContent("form-data", "xYzZY"); // Note: (multipart/form-data) mulpart is not required see implementation.
            multiPartContent2.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
            multiPartContent2.Headers.ContentDisposition.Name = "file";
            multiPartContent2.Headers.ContentDisposition.FileName = "subtitle.srt";
            multiPartContent2.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            multiPartContent2.Add(stringContent);
            // Add boundary.

            // Instantiate formdate with default boundary.
            var formData = new MultipartFormDataContent("xYzZY");
            //formData.Headers.ContentType = new MediaTypeHeaderValue("form-data");
            // TODO: formData.Headers.ContentLength = 10000;
            formData.Add(multiPartContent1);
            formData.Add(multiPartContent2);
            var response = await _httpClient.PostAsync(fullUrl, formData);
            // _httpClient
            return response.IsSuccessStatusCode;

            // TODO: How to prevent adding boundary when it's not neccessary!
            // SOLUTIONS:
            // Derive from MultipartContent and modify it by allowing stinng.Empty as boundary.
        }

        private static string GetFormatedBody(string movieHash, string subtitleContent)
        {
            // PAYLOAD
            string boundaryFormat = @"--xYzZY
Content-Disposition: form-data; name=""hash""

{0}
--xYzZY
Content-Disposition: form-data; name=""file""; filename=""{0}.srt""
Content-Type: application/octet-stream
Content-Transfer-Encoding: binary

{1}

--xYzZY
";
            return string.Format(boundaryFormat, movieHash, subtitleContent);
        }
    }
}
