using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

namespace SubDBSharp.Http
{
    public class Response : IResponse
    {
        public Response(HttpStatusCode statusCode, object body, IDictionary<string, string> headers)
        {
            StatusCode = statusCode;
            Body = body;
            Headers = new ReadOnlyDictionary<string, string>(headers);
        }
        public object Body { get; }
        public IReadOnlyDictionary<string, string> Headers { get; }
        public HttpStatusCode StatusCode { get; }
    }
}
