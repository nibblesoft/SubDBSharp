using SubDbSharp.Models;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace SubDbSharp
{
    public static class Utils
    {
        public static string FormatUserAgent(ProductHeaderValue procInfo)
        {
            return string.Format("{0} ({1} {2})", "SubDB/1.0", procInfo, "http://github.com/ivandrofly");
        }

        private static readonly StringBuilder _hexBuilder = new StringBuilder();

        public static string GetHashString(string file)
        {
            int bytesToRead = 64 * 1024;
            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var md5 = System.Security.Cryptography.MD5.Create())
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

        public static string ToHexadecimal(byte[] bytes)
        {
            _hexBuilder.Length = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                _hexBuilder.Append(bytes[i].ToString("x2"));
            }
            return _hexBuilder.ToString();
        }

        internal static HttpRequestMessage BuildRequestMessage(Request request, string endPoint, Uri subDBApiUrl)
        {
            // PAYLOAD
            string boundaryFormat = @"--xYzZY
Content-Disposition: form-data; name=""hash""

{0}
--xYzZY
Content-Disposition: form-data; name=""file""; filename=""{0}.srt""
Content-Type: application/octet-stream
Content-Transfer-Encoding: binary

{1}

--xYzZY
";
            // Uri
            Uri uri = new Uri(string.Format(endPoint, request.MovieHash), UriKind.Relative);
            Uri fullUrl = new Uri(subDBApiUrl, uri);

            string body = string.Format(boundaryFormat, request, request.Content);
            StringContent stringContent = new StringContent(body, Encoding.UTF8);
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
            stringContent.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("boundary", "xYzZY"));

            // Request message
            return new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = fullUrl,
                Content = stringContent
            };
        }

    }
}
