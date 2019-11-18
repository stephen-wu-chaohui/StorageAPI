using System;
namespace Storage.Core
{
    public interface IUploadItemStatus
    {
        public IUploadItem Source { get; }
        public UploadItemStatsCode StatusCode { get; }
        public Exception CloudServiceException { get; }
    }
}
