using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Moq;
using Storage.Core;
using Xunit;

namespace Storage.S3.Tests
{
    public class UnitTest1
    {
        private const string BucketName = "socketName";
        private const string notExistingBucket = "notExistingBucket";

        private List<S3Object> files = new List<S3Object> {
            new S3Object { Key="Key1", Owner = new Owner() { DisplayName = "Owner-1" } },
            new S3Object { Key="Key2", Owner = new Owner() { DisplayName = "Owner-1" } },
            new S3Object { Key="Key3", Owner = new Owner() { DisplayName = "Owner-1" } },
            new S3Object { Key="Key5", Owner = new Owner() { DisplayName = "Owner-1" } },
            new S3Object { Key="Key6", Owner = new Owner() { DisplayName = "Owner-1" } },
        }; 

        [Fact]
        public async Task Test1Async()
        {
            var S3Client = new Mock<IAmazonS3>();

            S3Client.Setup(x => x.DoesS3BucketExistAsync(It.IsAny<string>()))
                .ReturnsAsync(() => true);
            S3Client.Setup(x => x.DoesS3BucketExistAsync(notExistingBucket))
                .ReturnsAsync(() => false);

            S3Client.Setup(x => x.ListObjectsAsync(notExistingBucket, default))
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
            S3Client.Setup(x => x.ListObjectsAsync(It.IsAny<string>(), default))
                .ReturnsAsync(() => new ListObjectsResponse
                {
                    HttpStatusCode = HttpStatusCode.OK,
                    S3Objects = files
                });

            IFileStorage fileStorage = new S3FileStorage(S3Client.Object);

            await fileStorage.SetBucketAsync(BucketName);
            var listResponse = await fileStorage.ListAsync();
            Assert.Equal(listResponse.HttpStatusCode, HttpStatusCode.OK);

            await fileStorage.SetBucketAsync(notExistingBucket);
            listResponse = await fileStorage.ListAsync();
            Assert.Equal(listResponse.HttpStatusCode, HttpStatusCode.NotFound);
        }
    }
}
