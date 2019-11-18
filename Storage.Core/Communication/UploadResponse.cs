using System.Collections.Generic;
using System.Net;

namespace Storage.Core
{
    public class UploadResponse : IResponse
    {
        public ServiceStatusCode StatusCode { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public IList<IUploadItem> UploadedItems { get; set; }
        public IList<IUploadItemStatus> FailedItems { get; set; }
    }
}
