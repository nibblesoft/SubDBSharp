using System.Globalization;

namespace DesktopClient.Models
{
    public class LanguageItem
    {
        public CultureInfo CultureInfo { get; }
        public LanguageItem(string name)
        {
            CultureInfo = new CultureInfo(name);
        }

        public override string ToString() => CultureInfo.EnglishName;
    }
}
