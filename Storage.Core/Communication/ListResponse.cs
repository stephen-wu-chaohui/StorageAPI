using System.Collections.Generic;
using System.Net;

namespace Storage.Core
{
    public struct ListResponse : IResponseBase
    {
        public IEnumerable<IFileItem> Files;

        public HttpStatusCode HttpStatusCode { get; set; }
    }
}