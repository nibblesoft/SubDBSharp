using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubDBSharp
{
    public class Client
    {
        public string Name { get; private set; }
        public string Version { get; private set; }
        public Uri Url { get; private set; }

        public Client(string name, string version, Uri url)
        {
            Name = name;
            Version = version;
            Url = url;
        }

        public Client(string name, string version, string url) :
            this(name, version, new Uri(url))
        {
        }

    }
}
