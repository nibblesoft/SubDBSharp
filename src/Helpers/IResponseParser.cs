using System.Collections.Generic;

namespace SubDBSharp.Helpers
{
    public interface IResponseParser
    {
        IReadOnlyList<Language> ParseGetAvailablesLanguages(string response);
        IReadOnlyList<Language> ParseSearchSubtitle(string response, bool getVersions);
    }
}
