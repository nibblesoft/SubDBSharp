using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

namespace SubDBSharp.Http
{
    public class Response
    {
        public Response(HttpStatusCode statusCode, string body, IDictionary<string, string> headers)
        {
            StatusCode = statusCode;
            Body = body;
            Headers = new ReadOnlyDictionary<string, string>(headers);
        }
        public string Body { get; }
        public IReadOnlyDictionary<string, string> Headers { get; }
        public HttpStatusCode StatusCode { get; }
        public string ContentType { get; }
    }
}
