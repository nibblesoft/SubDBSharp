using System;
using System.Collections.Generic;

namespace SubDBSharp
{
    public class AvailableLanguages : ApiClient, IAvailableLanguages
    {
        public AvailableLanguages(ApiConnection apiConnection) :
            base(apiConnection)
        {

        }

        /// <summary>
        /// Returns list of all available languages.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<Language> Get()
        {
            // If null use default baseaddress when calling endpoint.
            ApiConnection.Get<IReadOnlyList<Language>>(null, );
        }
    }
}
