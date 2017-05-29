using System.Globalization;

namespace SubDBSharp
{
    public class Language
    {
        public Language(string name)
            : this(name, 0)
        {
        }

        public Language(string name, int count)
        {
            Culture = new CultureInfo(name);
            Name = Culture.TwoLetterISOLanguageName;
            Count = count;
        }

        public CultureInfo Culture { get; }
        public int Count { get; }
        public string Name { get; }

        public override string ToString() => Name;

        public string ToStringVerbose() => $"{Culture.DisplayName}, Versions: {Count}";

    }
}
