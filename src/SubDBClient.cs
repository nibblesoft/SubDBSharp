using SubDBSharp.Helpers;
using SubDBSharp.Http;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SubDBSharp
{
    public class SubDBClient : IDisposable, ISubDBClient
    {
        private readonly SubDBApi _subDbApi;
        private readonly IResponseParser _responseParser;

        #region Constuctors

        public SubDBClient(ProductHeaderValue productInformation)
            : this(productInformation, ApiUrls.SubDBApiUrl)
        {
        }

        public SubDBClient(ProductHeaderValue productInformation, Uri baseAddress)
        {
            baseAddress = baseAddress ?? ApiUrls.SubDBApiUrl;
            _subDbApi = new SubDBApi(productInformation, baseAddress);
            _responseParser = new CsvResponseParser();
        }

        #endregion

        /// <summary>
        /// Lists the languages of all subtitles currenttly available in database.
        /// </summary>
        /// <returns></returns>
        public Task<Response> GetAvailableLanguagesAsync() => _subDbApi.GetAvailableLanguagesAsync();

        /// <summary>
        /// To list the available subtitle languages for a given hash, a HTTP GET request will be made to the server.
        /// </summary>
        /// <param name="hash">The parameter hash is the hash of the video file, generated using our hash function.</param>
        /// <param name="getVersions">This parameter is optional. Using it to return how many versions per language of a subtitle we have in our database.</param>
        /// <returns></returns>
        public Task<Response> SearchSubtitleAsync(string hash, bool getVersions) => _subDbApi.SearchSubtitle(hash, getVersions);

        /// <summary>
        /// To download a subtitle, a HTTP GET request will be made to the server.
        /// </summary>
        /// <param name="hash">The parameter hash is the hash of the video file, generated using our hash function.</param>
        /// <param name="languages"> The parameter language can be a single language code (ex.: us), or a comma separated list in order of priority (ex.: us,nl).
        /// //When using a comma separated list, the first subtitle found is returned.</param>
        /// <returns></returns>
        public Task<Response> DownloadSubtitleAsync(string hash, params string[] languages) => _subDbApi.DownloadSubtitle(hash, languages);

        /// <summary>
        /// To upload a subtitle, a HTTP POST request will be made to the server.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<Response> UploadSubtitleAsync(string subtitle, string movie) => _subDbApi.UploadSubtitle(subtitle, movie);

        public void Dispose()
        {
            _subDbApi?.Dispose();
        }
    }
}