using System;
using System.Collections.Generic;
using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Moq;
using Xunit;

namespace Storage.S3.Tests
{
    [Collection("UnitTestsForUpload")]
    public class UnitTestsForUpload : UploadTests
    {
        public UnitTestsForUpload()
        {
        }

        public override string BucketName => "ExistingBucket";

        public override string NotExistingBucket => "NotExistingBucket";

        public override IAmazonS3 TestingS3Client
        {
            get
            {
                var S3Client = new Mock<IAmazonS3>();

                S3Client.Setup(x => x.DoesS3BucketExistAsync(It.IsAny<string>()))
                    .ReturnsAsync(() => true);
                S3Client.Setup(x => x.DoesS3BucketExistAsync(NotExistingBucket))
                    .ReturnsAsync(() => false);

                S3Client.Setup(x => x.ListObjectsAsync(NotExistingBucket, default))
                    .ReturnsAsync(() => new ListObjectsResponse
                    {
                        HttpStatusCode = HttpStatusCode.NotFound,
                        S3Objects = new List<S3Object>()
                    });
                S3Client.Setup(x => x.ListObjectsAsync((string)null, default))
                    .ReturnsAsync(() => new ListObjectsResponse
                    {
                        HttpStatusCode = HttpStatusCode.NotFound,
                        S3Objects = new List<S3Object>()
                    });
                return S3Client.Object;
            }
        }
    }
}
