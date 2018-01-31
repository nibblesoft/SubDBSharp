using System;
using System.Net.Http;

namespace SubDBSharp.Models
{
    public class Request : IRequest
    {
        public Request(Uri endpoint, HttpMethod method, HttpContent content)
        {
            EndPoint = endpoint;
            Method = method;
            Body = content;
        }

        public Uri EndPoint { get; }
        public HttpMethod Method { get; }
        public HttpContent Body { get; }
    }
}
