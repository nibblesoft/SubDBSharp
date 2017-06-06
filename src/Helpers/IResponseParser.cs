using System;
using System.Collections.Generic;
using System.Text;

namespace SubDBSharp.Helpers
{
    public interface IResponseParser
    {
        IReadOnlyList<Language> ParseGetAvailablesLanguages(string response);
        IReadOnlyList<Language> ParseSearchSubtitle(string response, bool getVersions);
    }
}
