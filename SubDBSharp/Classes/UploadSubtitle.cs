using System;
namespace SubDBSharp
{
    public class UploadSubtitle : ApiClient, IUploadSubtitle
    {
        public UploadSubtitle(ApiConnection apiConnection) :
            base(apiConnection)
        {

        }
        public void Upload(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
