using System.Collections.Generic;

namespace SubDBSharp
{
    public interface IAvailableLanguages
    {
        IReadOnlyList<Language> Get();
    }
}
