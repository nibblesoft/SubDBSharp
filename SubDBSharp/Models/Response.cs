using System;
using System.Net.Http;

namespace SubDBSharp
{
    public class Response : IDisposable
    {
        public StreamContent StreamContent { get; }
        public string FileName { get; }

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
