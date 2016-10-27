namespace SubDBSharp
{
    public class AvailableSubtitle : IAvailableSubtitle
    {
        public int Count { get; }
        public Language Language { get; }
        public string SubtitleHash { get; }

        public AvailableSubtitle(int count, string hash, Language language)
        {
            Count = count;
            SubtitleHash = hash;
            Language = language;
        }
    }
}
