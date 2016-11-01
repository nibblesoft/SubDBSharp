using System;
using System.Net.Http;

namespace SubDBSharp
{
    public class Response : IDisposable
    {
        public StreamContent StreamContent { get; private set; }
        public string FileName { get; private set; }

        public Response(string fileName, StreamContent streamContent)
        {
            FileName = fileName;
            StreamContent = streamContent;
        }

        public void Dispose()
        {
            StreamContent?.Dispose();
        }
    }
}
