using System.Collections.Generic;

namespace SubDBSharp
{
    public interface ISearchSubtitles
    {
        IReadOnlyList<AvailableSubtitle> Search(string hash);
        IReadOnlyList<AvailableSubtitle> Search(string hash, Language languages);
    }
}
