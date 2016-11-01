using System;
using System.Net.Http.Headers;

namespace SubDBSharp
{
    /// <summary>
    /// TODO: Change to ProductHeaderValue?
    /// </summary>
    public class Client
    {
        private readonly ProductHeaderValue _productHeaderValue;

        public Uri Url { get; private set; }

        public ProductHeaderValue ProductHeaderValue
        {
            get
            {
                return _productHeaderValue;
            }
        }

        public Client(string name, string version, string url) :
            this(name, version, new Uri(url))
        {
        }

        public Client(string name, string version, Uri url)
            : this(new ProductHeaderValue(name, version), url)
        {

        }

        public Client(ProductHeaderValue productHeaderValue, Uri url)
        {
            _productHeaderValue = productHeaderValue;
            Url = url;
        }

        public override string ToString()
        {
            return $"({_productHeaderValue.Name}/{_productHeaderValue.Version:0.00}; {Url})";
        }
    }
}
