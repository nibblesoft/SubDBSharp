using System.Globalization;

namespace SubDBSharp
{
    public class Language
    {
        public CultureInfo Culture { get; }
        public int Count { get; private set; }
        public string Name { get; private set; }

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

        public override string ToString()
        {
            return $"{Culture.DisplayName}, Versions: {Count}";
        }
    }
}
