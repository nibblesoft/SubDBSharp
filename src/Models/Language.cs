namespace SubDBSharp
{
    public class Language : ILanguage
    {
        public Language(string name) : this(name, 0)
        {
        }

        public Language(string name, int count)
        {
            Name = name;
            Count = count;
        }

        public int Count { get; }
        public string Name { get; }
        public override string ToString() => Name;
    }
}
