using System;
using System.IO;
using System.Net.Http;

namespace SubDBSharp
{
    public class Response : IDisposable
    {
        public Stream SubStream { get; }
        public string FileName { get; }

        public Response(string fileName, Stream streamContent)
        {
            FileName = fileName;
            SubStream = streamContent;
        }

        public void Dispose()
        {
            SubStream?.Dispose();
        }
    }
}
