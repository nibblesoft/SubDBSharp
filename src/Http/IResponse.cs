using System.Collections.Generic;
using System.Net;

namespace SubDBSharp.Http
{
    public interface IResponse
    {
        object Body { get; }
        IReadOnlyDictionary<string, string> Headers { get; }
        HttpStatusCode StatusCode { get; }
    }
}