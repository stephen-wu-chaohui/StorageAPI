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
    [Collection("IntegrationTestsForUpload")]
    public class IntegrationTestsForUpload : UploadTests
    {
        public override string BucketName => "for-pushpay-doc";

        public override string NotExistingBucket => "NotExistingBucket";

        public override IAmazonS3 TestingS3Client
        {
            get
            {
                var s3Client = new AmazonS3Client(
                    "AKIAXGUNRM56GBIWVZNS",
                    "tKlhP/lW/kpycNWACvrzJzJPY/DhByqyuqdfzPXw",
                    Amazon.RegionEndpoint.USWest2
                );
                return s3Client;
            }
        }
    }
}
