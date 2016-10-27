using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubDBSharp
{
    public abstract class ApiClient
    {
        protected IConnection Connection { get; private set; }
        protected IApiConnection ApiConnection { get; private set; }

        protected ApiClient(ApiConnection apiConnection)
        {
            ApiConnection = apiConnection;
            Connection = apiConnection.Connection;
        }
    }
}
