using System;

namespace Storage.Core
{
    [Serializable]
    public class CloudStorageException : Exception
    {
        public CSCode ErrorCode { get; }

        public CloudStorageException(CSCode errorCode, string message, Exception exception = null)
            : base(message, exception)
        {
            ErrorCode = errorCode;
        }

    }

    public enum CSCode
    {
        OK,
        InvalidSettings,
        InvalidBucketName,
        BucketDoesNotExist,
        BucketAccessDenied,
        NoFilesToUpload,
        FileDoesNotExist,
        FileAccessDenied,
        CloudServiceException,
        GeneralException,
    };
}
