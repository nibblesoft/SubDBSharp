using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SubDBSharp
{
    public class ApiConnection : IApiConnection
    {
        public IConnection Connection { get; private set; }

        public ApiConnection(IConnection connection)
        {
            Connection = connection;
        }

        public async Task<T> Get<T>(Uri url)
        {
            return await Get<T>(url, null);
        }

        public async Task<T> Get<T>(Uri url, Dictionary<string, string> parameters)
        {
            return await Connection.GetAsync<T>(url, parameters);
        }

        public async Task<T> Post<T>(Uri url, object data, object body)
        {
            return await Post<T>(url, data, body, null);
        }

        public async Task<T> Post<T>(Uri url, object data, object body, string accept)
        {
            return await Post<T>(url, data, body, null, null);
        }

        public async Task<T> Post<T>(Uri url, object data, object body, string accept, string contentType)
        {
            // TODO: pass correct param!
            return await SendData<T>(url, null, body, accept, contentType);
        }

        // Send data
        async Task<T> SendData<T>(Uri uri, HttpMethod method, object body, string accepts, string contentType)
        {
            return default(T);
        }

        async Task<T> Run<T>(Uri url)
        {
            return default(T);
        }

        // Every http method will 
        async Task<T> RunRequest<T>(IRequest request)
        {
            return default(T);
        }


    }
}
