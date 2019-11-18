using System.Collections.Generic;
using System.Net;

namespace Storage.Core
{
    public struct ListResponse : IResponse
    {
        public ServiceStatusCode StatusCode { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public IEnumerable<IFileItem> Files { get; set; }
    }
}