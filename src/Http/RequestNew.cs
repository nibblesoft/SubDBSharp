﻿using System;
using System.Collections.Generic;
using System.Net.Http;

namespace SubDBSharp.Http
{
    public class RequestNew
    {
        public RequestNew()
        {
            Headers = new Dictionary<string, string>();
            Parameters = new Dictionary<string, string>();
            Timeout = TimeSpan.FromSeconds(100);
        }

        public object Body { get; set; }
        public Dictionary<string, string> Headers { get; private set; }
        public HttpMethod Method { get; set; }
        public Dictionary<string, string> Parameters { get; private set; }
        public Uri BaseAddress { get; set; }
        public Uri Endpoint { get; set; }
        public TimeSpan Timeout { get; set; }
        public string ContentType { get; set; }
    }

}
