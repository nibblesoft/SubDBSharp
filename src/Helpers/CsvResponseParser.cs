using System.Collections.Generic;

namespace SubDBSharp.Helpers
{
    public class CsvResponseParser : IResponseParser
    {
        public IReadOnlyList<Language> ParseGetAvailablesLanguages(string response)
        {
            var readonlyList = new List<Language>();
            foreach (var isoTwoLetter in response.Split(','))
            {
                if (!string.IsNullOrWhiteSpace(isoTwoLetter)) readonlyList.Add(new Language(isoTwoLetter));
            }
            return readonlyList;
        }

        public IReadOnlyList<Language> ParseSearchSubtitle(string response, bool getVersions)
        {
            var listLanguages = new List<Language>();
            foreach (var token in response.Split(','))
            {
                // Tokens are available only if getVersion == true
                var tokens = token.Split(':');
                var lang = getVersions ? new Language(tokens[0], int.Parse(tokens[1])) : new Language(tokens[0]);
                listLanguages.Add(lang);
            }
            return listLanguages;
        }
    }
}
