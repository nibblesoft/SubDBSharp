using System.Runtime.InteropServices;

namespace DesktopClient.DetectEncoding.Multilang
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct _ULARGE_INTEGER
    {
        public ulong QuadPart;
    }
}
