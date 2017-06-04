using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SubDbSharp
{
    public static class UriExtensions
    {
        public static Uri ApplyParameters(this Uri uri, IDictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.Any()) return uri;

            // to prevent values being persisted across requests
            // use a temporary dictionary which combines new and existing parameters
            IDictionary<string, string> p = new Dictionary<string, string>(parameters);

            string queryString;
            if (uri.IsAbsoluteUri)
            {
                queryString = uri.Query;
            }
            else
            {
                var hasQueryString = uri.OriginalString.IndexOf("?", StringComparison.Ordinal);
                queryString = hasQueryString == -1
                    ? ""
                    : uri.OriginalString.Substring(hasQueryString);
            }

            var values = queryString.Replace("?", "")
                                    .Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

            var existingParameters = values.ToDictionary(
                        key => key.Substring(0, key.IndexOf('=')),
                        value => value.Substring(value.IndexOf('=') + 1));

            foreach (var existing in existingParameters)
            {
                if (!p.ContainsKey(existing.Key))
                {
                    p.Add(existing);
                }
            }

            Func<string, string, string> mapValueFunc = (key, value) => key == "q" ? value : Uri.EscapeDataString(value);

            string query = string.Join("&", p.Select(kvp => kvp.Key + "=" + mapValueFunc(kvp.Key, kvp.Value)));
            if (uri.IsAbsoluteUri)
            {
                var uriBuilder = new UriBuilder(uri)
                {
                    Query = query
                };
                return uriBuilder.Uri;
            }

            return new Uri(uri + "?" + query, UriKind.Relative);
        }

        public static Uri ApplySearchSubtitleParameters(this Uri uri, string hash, bool getVersions)
        {
            string query = $"action=search&hash={hash}";
            if (getVersions)
            {
                query += $"&versions";
            }
            var uriBuilder = new UriBuilder(uri)
            {
                Query = query
            };
            return uriBuilder.Uri;
        }

        public static Uri ApplyDownloadSubtitleParameters(this Uri uri, string hash, params string[] languages)
        {
            // en,es,fr,it,nl,pl,pt,ro,sv,tr

            var sb = new StringBuilder($"action=download&hash={hash}&language=");
            string query = sb.ToString() + string.Join(",", languages);
            var uriBuilder = new UriBuilder(uri)
            {
                Query = query
            };
            return uriBuilder.Uri;
        }
    }
}
