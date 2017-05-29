using System;
using System.Globalization;

namespace SubDBSharp
{
    public static class StringExtensions
    {
        public static Uri FormatUri(this string pattern, params object[] args)
        {
            return new Uri(string.Format(CultureInfo.InvariantCulture, pattern, args), UriKind.Relative);
        }
    }
}
