using System;
using System.Net.Http;

namespace SubDBSharp.Models
{
    public interface IRequest
    {
        HttpContent Body { get; }
        Uri EndPoint { get; }
        HttpMethod Method { get; }
    }
}