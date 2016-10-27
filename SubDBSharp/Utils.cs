using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubDBSharp
{
    public static class Utils
    {
        private static readonly StringBuilder _hexBuilder = new StringBuilder();

        public static string GetHashString(string file)
        {
            int bytesToRead = 64 * 1024;
            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                int len = (int)fs.Length;
                byte[] buffer = new byte[bytesToRead * 2];

                // Read first 64kb.
                fs.Read(buffer, 0, bytesToRead);

                // Read last 64kb.
                fs.Seek(-bytesToRead, SeekOrigin.End);
                fs.Read(buffer, bytesToRead, bytesToRead);
                byte[] computedBuffer = md5.ComputeHash(buffer);
                return ToHexadecimal(computedBuffer);
            }
        }

        private static string ToHexadecimal(byte[] bytes)
        {
            _hexBuilder.Length = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                _hexBuilder.Append(bytes[i].ToString("x2"));
            }
            return _hexBuilder.ToString();
        }
    }
}
