﻿using System.Runtime.InteropServices;

namespace DesktopClient.DetectEncoding.Multilang
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct _FILETIME
    {
        public uint dwLowDateTime;
        public uint dwHighDateTime;
    }
}
