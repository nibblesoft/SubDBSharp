using System;
using System.IO;
using System.Net;
using System.Text;

namespace SubDBSharp
{
    public class SubDBClient
    {
        private const string ProtocolName = "SubDB";
        private const string PrtocolVersion = "1.0";
        private readonly Uri _baseAddress;
        private readonly string _userAgent;

        public SubDBClient(Client client)
            : this(new Uri("http://api.thesubdb.com/"), client)
        {
        }

        public SubDBClient(Uri _endPoint, Client client)
        {
            _baseAddress = _endPoint;
            _userAgent = string.Format("{0}/{1} ({2}/{3}; {4})",
                ProtocolName, PrtocolVersion, client.Name, client.Version, client.Url);
        }


        public string LanguagesAvailable()
        {
            string action = "?action=languages";
            return ReadResponseStream(RunRequestGet(action, WebRequestMethods.Http.Get).GetResponse());
        }

        public string SearchSubtitle(string hash, string version = "")
        {
            string action = $"?action=search&hash={hash}";
            if (version.Length > 0)
                action += $"&version={version}";
            return ReadResponseStream(RunRequestGet(action, WebRequestMethods.Http.Get).GetResponse());
        }

        public Stream DownloadSubtitle(string hash, string language)
        {
            string action = $"?action=download&hash={hash}&language={language}";
            var ms = new MemoryStream();
            Stream r = RunRequestGet(action, WebRequestMethods.Http.Get).GetResponse().GetResponseStream();
            int buffSize = 1024;
            byte[] buffer = new byte[buffSize];
            int bytesRead = 0;
            int totalReadBytes = 0;
            while ((bytesRead = r.Read(buffer, 0, buffSize)) > 0)
            {
                ms.Write(buffer, 0, bytesRead);
                totalReadBytes += bytesRead;
            }
            var buff = ms.ToArray();
            Array.Clear(buff, totalReadBytes, (int)ms.Length - totalReadBytes);
            return new MemoryStream(buff);
        }

        public string UploadSubtitle(string movieFile, string subfile)
        {
            string movieHash = Utils.GetHashString(movieFile);
            string contentBody = GetFormatedBody(movieHash, File.ReadAllText(subfile, Encoding.UTF8));
            byte[] textBytes = Encoding.UTF8.GetBytes(contentBody);
            string action = $"?action=upload&hash={movieHash}";
            HttpWebRequest request = RunRequestGet(action, WebRequestMethods.Http.Post);
            request.ContentType = "multipart/form-data; boundary=xYzZY";
            request.Headers.Add(HttpRequestHeader.Pragma, "no-cache");
            request.ContentLength = textBytes.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(textBytes, 0, textBytes.Length);
            requestStream.Dispose();
            try
            {
                using (WebResponse responseStream = request.GetResponse())
                {
                    return ReadResponseStream(responseStream);
                }
            }
            catch (WebException ex)
            {
                return ex.Message;
            }
            finally
            {
                if (requestStream != null)
                {
                    requestStream.Close();
                    requestStream.Dispose();
                }
            }
        }

        private static string GetFormatedBody(string movieHash, string subtitleContent)
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
            return string.Format(boundaryFormat, movieHash, subtitleContent.TrimEnd());
        }

        public HttpWebRequest RunRequestGet(string action, string method)
        {
            var request = (HttpWebRequest)WebRequest.Create(_baseAddress + action);
            request.UserAgent = _userAgent;
            request.Method = method;
            return request;
        }

        private static string ReadResponseStream(WebResponse reponseStream)
        {
            using (var sr = new StreamReader(reponseStream.GetResponseStream()))
            {
                return sr.ReadToEnd();
            }
        }

    }
}
