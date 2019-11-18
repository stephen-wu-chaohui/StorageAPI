using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Moq;
using Storage.Core;
using Xunit;

namespace Storage.S3.Tests
{
    public abstract class UploadTests
    {
        public abstract string BucketName { get; }
        public abstract string NotExistingBucket { get; }
        public abstract IAmazonS3 TestingS3Client { get; }
    
        [Theory, InlineData(null), InlineData("")]
        public async Task TestUploadInvalidSocketAsync(string invalidBucketName)
        {
            IAmazonS3 S3Client = TestingS3Client;
            ICloudStorage sut = new S3CloudStorage(S3Client);
            var fileList = PrepareUploadItems().ToList();

            UploadResponse uploadResponse = await sut.UploadAsync(invalidBucketName, fileList);
            Assert.Equal(ServiceStatusCode.InvalidBucketName, uploadResponse.StatusCode);
        }

        [Fact]
        public async Task TestUploadToNotExistingBucketAsync()
        {
            IAmazonS3 S3Client = TestingS3Client;
            ICloudStorage fileStorage = new S3CloudStorage(S3Client);
            var fileList = PrepareUploadItems().ToList();

            UploadResponse uploadResponse = await fileStorage.UploadAsync(NotExistingBucket, fileList);
            Assert.Equal(ServiceStatusCode.BucketDoesNotExist, uploadResponse.StatusCode);
        }

        [Fact]
        public async Task UploadTestsAsync()
        {
            IAmazonS3 S3Client = TestingS3Client;
            ICloudStorage sut = new S3CloudStorage(S3Client);
            var uploadItems = PrepareUploadItems().ToList();

            UploadResponse uploadResponse = await sut.UploadAsync(BucketName, uploadItems);

            Assert.Equal(ServiceStatusCode.OK, uploadResponse.StatusCode);
            Assert.Equal(2, uploadResponse.UploadedItems.Count);
            Assert.Collection(uploadResponse.UploadedItems,
                item => Assert.Contains("First/1.jpg", item.KeyName),
                item => Assert.Contains("Second/2.jpg", item.KeyName));

            Assert.Equal(3, uploadResponse.FailedItems.Count);

            foreach (var fitem in uploadResponse.FailedItems)
            {
                switch (fitem.Source.KeyName)
                {
                    case "":
                    case null:
                        Assert.Equal(UploadItemStatsCode.InvalidKeyName, fitem.StatusCode);
                        break;
                    case "Well/Good.key":
                        Assert.Equal(UploadItemStatsCode.IOException, fitem.StatusCode);
                        break;
                }
            }
        }

        private IUploadItem PrepareItemFromFile(string keyName, string pathName)
        {
            Mock<IUploadItem> mock = new Mock<IUploadItem>();
            mock.SetupGet(ff => ff.KeyName).Returns(keyName);
            mock.Setup(ff => ff.GetStream()).Returns(() => new FileStream(pathName, FileMode.Open));
            return mock.Object;
        }

        private IEnumerable<IUploadItem> PrepareUploadItems()
        {
            yield return PrepareItemFromFile("First/1.jpg", @"/Users/stephen/Documents/ToUpload/1.jpg");
            yield return PrepareItemFromFile("Second/2.jpg", @"/Users/stephen/Documents/ToUpload/2.jpg");
            yield return PrepareItemFromFile("", "BB.path");
            yield return PrepareItemFromFile(null, "CC.path");
            yield return PrepareItemFromFile("Well/Good.key", "Bad.path");
        }
    }
}
