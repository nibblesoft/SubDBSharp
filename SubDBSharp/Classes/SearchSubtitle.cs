using SubDBSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubDBSharp
{
    public class SearchSubtitle : ApiClient, ISearchSubtitles
    {
        // 'http://api.thesubdb.com/?action=search&hash=edc1981d6459c6111fe36205b4aff6c2[&versions]' (GET)
        private readonly Uri _searchSubtitleUrl = new Uri("action=search", UriKind.Relative);

        public SearchSubtitle(ApiConnection apiConnection) : base(apiConnection)
        {

        }

        /// <summary>
        /// Takes movie hash and return list of available language.
        /// </summary>
        /// <param name="hash">Movie hash</param>
        /// <returns></returns>
        public IReadOnlyList<AvailableSubtitle> Search(string hash) // TODO: Rename this ByHash(string hash)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Takes movie hash and return list of avaible language and total available for each language.
        /// </summary>
        /// <param name="hash">Movie hash</param>
        /// <param name="languages"></param>
        /// <returns></returns>
        public IReadOnlyList<AvailableSubtitle> Search(string hash, Language languages)
        {
            throw new NotImplementedException();
        }
    }
}
