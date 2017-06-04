using System.Globalization;

namespace SubDbSharp
{
    public class Language
    {
        public Language(string name) : this(name, 0)
        {
        }

        public Language(string name, int count)
        {
            Name = Culture.TwoLetterISOLanguageName;
            Count = count;
        }

        public CultureInfo Culture { get; }
        public int Count { get; }
        public string Name { get; }
        public override string ToString() => Name;
    }
}
