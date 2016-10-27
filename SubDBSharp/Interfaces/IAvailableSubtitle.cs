namespace SubDBSharp
{
    public interface IAvailableSubtitle
    {
        int Count { get; }
        Language Language { get; }
        string SubtitleHash { get; }
    }
}