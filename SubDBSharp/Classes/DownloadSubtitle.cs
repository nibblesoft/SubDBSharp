using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubDBSharp
{
    public class DownloadSubtitle : ApiClient, IDownloadSubtitle
    {
        public DownloadSubtitle(ApiConnection apiConnection) :
            base(apiConnection)
        {

        }

        public void Download(string hash, Language language)
        {
            throw new NotImplementedException();
        }

        public void Download(string hash, IReadOnlyList<Language> listLanguages)
        {
            throw new NotImplementedException();
        }

    }
}
