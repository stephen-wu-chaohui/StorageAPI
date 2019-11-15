using System.Net;

namespace Storage.Core
{
    public class SetBucketResponse : IResponseBase
    {
        public HttpStatusCode HttpStatusCode { get; set; }
    }
}