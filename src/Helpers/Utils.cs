using System.IO;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace SubDBSharp.Helpers
{
    public static class Utils
    {
        public static string FormatUserAgent(ProductHeaderValue procInfo) => $"{"SubDB/1.0"} ({procInfo} {"http://github.com/ivandrofly"})";

        public static string GetMovieHash(string file)
        {
            int bytesToRead = 64 * 1024;
            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var md5 = MD5.Create())
            {
                // file too short
                if (fs.Length < bytesToRead) return string.Empty;

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

        public static string ToHexadecimal(byte[] bytes)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++) sb.Append(bytes[i].ToString("x2"));

            return sb.ToString();
        }

        public static bool IsUtf8(byte[] buffer)
        {
            // call this method to encoding;
            return false;
        }
    }
}