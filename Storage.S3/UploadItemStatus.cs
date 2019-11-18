using System;
using System.IO;
using Amazon.S3;
using Storage.Core;

namespace Storage.S3
{
    public class UploadItemStatus : IUploadItemStatus
    {
        public IUploadItem Source { get; }

        public UploadItemStatsCode StatusCode { get; }

        public Exception CloudServiceException { get; }

        public UploadItemStatus(IUploadItem source, AmazonS3Exception cloudServiceException)
        {
            Source = source;
            StatusCode = UploadItemStatsCode.CloudServiceException;
            CloudServiceException = cloudServiceException;
        }

        public UploadItemStatus(IUploadItem source, IOException ioException)
        {
            Source = source;
            StatusCode = UploadItemStatsCode.IOException;
            CloudServiceException = ioException;
        }

        public UploadItemStatus(IUploadItem source, Exception otherException)
        {
            Source = source;
            StatusCode = UploadItemStatsCode.GeneralException;
            CloudServiceException = otherException;
        }

        public UploadItemStatus(IUploadItem source, UploadItemStatsCode statusCode)
        {
            Source = source;
            StatusCode = statusCode;
        }
    }
}
