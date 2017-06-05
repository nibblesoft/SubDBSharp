using SubDbSharp.Helpers;
using SubDbSharp.Http;
using SubDbSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SubDbSharp
{
    public class SubDbClient
    {
        private readonly SubDbApi _subDbApi;
        private readonly IResponseParser _responseParser;

        #region Constuctors

        public SubDbClient(ProductHeaderValue productInformation)
            : this(productInformation, null)
        {
        }

        public SubDbClient(ProductHeaderValue productInformation, Uri baseAddress)
        {
            if (baseAddress == null)
            {
                _subDbApi = new SubDbApi(productInformation);
            }
            else
            {
                _subDbApi = new SubDbApi(productInformation, baseAddress);
            }
            _responseParser = new CsvResponseParser();
        }

        #endregion

        /// <summary>
        /// Lists the languages of all subtitles currenttly available in database.
        /// </summary>
        /// <returns></returns>
        public Task<Response> GetLanguagesAvailableAsync()
        {
            return _subDbApi.GetLanguagesAvailableAsync();
            //return _responseParser.ParseGetAvailablesLanguages(response);
        }

        // TODO: Remove async suffix in this method.
        /// <summary>
        /// To list the available subtitle languages for a given hash, a HTTP GET request will be made to the server.
        /// </summary>
        /// <param name="hash">The parameter hash is the hash of the video file, generated using our hash function.</param>
        /// <param name="getVersions">This parameter is optional. Using it to return how many versions per language of a subtitle we have in our database.</param>
        /// <returns></returns>
        public Task<Response> SearchSubtitleAsync(string hash, bool getVersions)
        {
            return _subDbApi.SearchSubtitle(hash, getVersions);
            //return _responseParser.ParseGetAvailablesLanguages(response);
        }

        /// <summary>
        /// To download a subtitle, a HTTP GET request will be made to the server.
        /// </summary>
        /// <param name="hash">The parameter hash is the hash of the video file, generated using our hash function.</param>
        /// <param name="languages"> The parameter language can be a single language code (ex.: us), or a comma separated list in order of priority (ex.: us,nl).
        /// //When using a comma separated list, the first subtitle found is returned.</param>
        /// <returns></returns>
        public Task<Response> DownloadSubtitleAsync(string hash, params string[] languages)
        {
            return _subDbApi.DownloadSubtitle(hash, languages);
        }

        /// <summary>
        /// To upload a subtitle, a HTTP POST request will be made to the server.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<Response> UploadSubtitleAsync(string subtitle, string movie)
        {
            return _subDbApi.UploadSubtitle(subtitle, movie);
        }

    }
}