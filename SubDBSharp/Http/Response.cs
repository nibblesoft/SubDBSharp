using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

namespace SubDbSharp.Http
{
    public class Response
    {
        public Response(HttpStatusCode statusCode, object body, IDictionary<string, string> headers, string file)
        {
            // remove if not in httpclient 
            StatusCode = statusCode;
            Body = body;
            Headers = new ReadOnlyDictionary<string, string>(headers);
            // TODO: Store file in header;
        }
        public object Body { get; set; }
        public IReadOnlyDictionary<string, string> Headers { get; }
        public HttpStatusCode StatusCode { get; }
        public string ContentType { get; }
    }
}
