namespace SubDBSharp
{
    public interface ILanguage
    {
        int Count { get; }
        string Name { get; }

        string ToString();
    }
}