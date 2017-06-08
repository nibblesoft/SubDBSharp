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
                var lang = new Language(isoTwoLetter);
                readonlyList.Add(lang);
            }
            return readonlyList;
        }

        public IReadOnlyList<Language> ParseSearchSubtitle(string response, bool getVersions)
        {
            var listLanguages = new List<Language>();
            foreach (var token in response.Split(','))
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
                {
                    listLanguages.Add(lang);
                }
            }
            return listLanguages;
        }
    }
}
