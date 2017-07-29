using System.Runtime.InteropServices;

namespace DesktopClient.DetectEncoding.Multilang
{
    [ComImport, Guid("275C23E1-3747-11D0-9FEA-00AA003F8646"), CoClass(typeof(CMultiLanguageClass))]
    public interface ICMultiLanguage : IMultiLanguage
    {
    }
}
