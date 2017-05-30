using System;

namespace SubDBSharp
{
    public static class ApiUrls
    {
        /// <summary>
        /// Uri used for development and testing.
        /// </summary>
        public static readonly Uri SubDbApiSandBoxUrl = new Uri("http://sandbox.thesubdb.com/", UriKind.Absolute);
        public static readonly Uri SubDBApiUrl = new Uri("http://api.thesubdb.com/", UriKind.Absolute);
    }
}
