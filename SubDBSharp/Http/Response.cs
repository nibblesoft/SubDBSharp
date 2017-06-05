using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

namespace SubDbSharp.Http
{
    public class Response
    {
        public Response(HttpStatusCode statusCode, string body, IDictionary<string, string> headers)
        {
            // remove if not in httpclient 
            StatusCode = statusCode;
            Body = body;
            Headers = new ReadOnlyDictionary<string, string>(headers);
            // TODO: Store file in header;
        }
        public string Body { get; }
        public IReadOnlyDictionary<string, string> Headers { get; }
        public HttpStatusCode StatusCode { get; }
        public string ContentType { get; }
    }
}
