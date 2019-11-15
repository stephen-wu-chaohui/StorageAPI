using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Microsoft.AspNetCore.Http;
using Storage.Core;

namespace Storage.S3
{
    public class S3FileStorage : IFileStorage
    {
        private readonly IAmazonS3 client;

        private string bucketName;

        public S3FileStorage(IAmazonS3 client)
        {
            this.client = client;
        }

        public async Task<UploadResponse> UploadAsync(IList<IFormFile> formFiles)
        {
            if (string.IsNullOrEmpty(bucketName))
            {
                return new UploadResponse { HttpStatusCode = HttpStatusCode.BadRequest };
            }
            foreach(var file in formFiles)
            {
                await client.UploadObjectFromStreamAsync(bucketName, file.Name, file.OpenReadStream(), null);
            }
            return new UploadResponse { HttpStatusCode = HttpStatusCode.OK };
        }

        public async Task<ListResponse> ListAsync()
        {
            if (string.IsNullOrEmpty(bucketName))
            {
                return new ListResponse { HttpStatusCode = HttpStatusCode.NotFound };
            }

            var s3Response = await client.ListObjectsAsync(bucketName);

            var response = new ListResponse
            {
                HttpStatusCode = s3Response.HttpStatusCode
            };

            if (s3Response.HttpStatusCode == HttpStatusCode.OK)
            {
                response.Files = s3Response.S3Objects.Select(s3Object =>
                {
                    IFileItem item = new FileItem
                    {
                        Key = s3Object.Key,
                        Owner = s3Object.Owner.DisplayName,
                        Size = s3Object.Size,
                    };
                    return item;
                });
            }
            return response;
        }
            
        public async Task<SetBucketResponse> SetBucketAsync(string bucketName)
        {
            bool bExisting = await client.DoesS3BucketExistAsync(bucketName);
            if (bExisting)
            {
                this.bucketName = bucketName;
                return new SetBucketResponse { HttpStatusCode = HttpStatusCode.OK };
            } else
            {
                this.bucketName = null;
                return new SetBucketResponse { HttpStatusCode = HttpStatusCode.NotFound };
            }
        }

    }
}
