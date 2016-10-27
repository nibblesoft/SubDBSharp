using System.Collections.Generic;

namespace SubDBSharp
{
    public interface IDownloadSubtitle
    {
        void Download(string hash, Language language);
        void Download(string hash, IReadOnlyList<Language> listLanguages);
    }
}
