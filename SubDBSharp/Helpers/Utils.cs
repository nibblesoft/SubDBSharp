using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace SubDBSharp
{
    public static class Utils
    {
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

        private static string ToHexadecimal(byte[] bytes)
        {
            _hexBuilder.Length = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                _hexBuilder.Append(bytes[i].ToString("x2"));
            }
            return _hexBuilder.ToString();
        }

        /*
        internal static string FormatUserAgent(ProductHeaderValue producInformation)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} ({1} {2}; {3}; {4}; SubDBSharp {5}",
                producInformation,
                Environment.OSVersion.Platform,
                Environment.OSVersion.Version.ToString(3),
                Environment.Is64BitOperatingSystem ? "amd64" : "x86",
                CultureInfo.CurrentCulture.Name,
                "1.0" // TODO: Get information from assembly.
                );
            //https://github.com/nibblesoft/SubDBSharp
        }
        */

        internal static HttpRequestMessage BuildRequestMessage(string movieFile, string subtitleFile, string endPoint, Uri subDBApiUrl)
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
            // Payloads
            string movieHash = Utils.GetHashString(movieFile);
            string subtitleContent = File.ReadAllText(subtitleFile).Trim();

            // Uri
            var uri = new Uri(string.Format(endPoint, movieHash), UriKind.Relative);
            var fullUrl = new Uri(subDBApiUrl, uri);

            string body = string.Format(boundaryFormat, movieHash, subtitleContent);
            var len = body.Length;
            var stringContent = new StringContent(body, Encoding.UTF8);

            // Request
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = fullUrl,
                Content = stringContent
            };

            // contentLen is equal to body.Length;
            var contentLen = stringContent.Headers.ContentLength;

            request.Content.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
            request.Content.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("boundary", "xYzZY"));
            request.Content.Headers.ContentLength = contentLen;
            return request;
        }

    }
}
