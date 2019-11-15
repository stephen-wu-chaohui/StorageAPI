using System.Net;

namespace Storage.Core
{
    public class UploadResponse : IResponseBase
    {
        public HttpStatusCode HttpStatusCode { get; set; }
    }
}
