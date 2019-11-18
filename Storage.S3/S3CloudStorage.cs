using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Storage.Core;

namespace Storage.S3
{
    /// <summary>
    /// Implementation class of IFileStorage using S3 buckets to store files
    /// </summary>
    public class S3CloudStorage : ICloudStorage
    {
        private readonly IAmazonS3 client;

        public S3CloudStorage(IAmazonS3 client)
        {
            this.client = client;
        }

        /// <summary>
        /// Store file stream to Amazon S3 buckets
        /// </summary>
        /// <param name="bucketName">The bucket name where uploaded files will be stored</param>
        /// <param name="uploadItems">File streams with key and metatadata to uploaded</param>
        /// <returns>
        /// UploadResponse contains:
        /// - StatusCode: an error code causing the method failed to run (OK if no error)
        /// - HttpStatusCode: The extra error code decribing the reason of failure when StatusCode is ServiceStatusCode.HttpStatusCode
        /// - UploadedItems: The items in uploadItems which were successfully uploaded to S3 buckets
        /// - FailedItems: The items in uploadItems which were failed to uploaded,
        ///     with a UploadItemResponseCode and a possible exception for the details of filure
        /// </returns>
        public async Task<UploadResponse> UploadAsync(string bucketName, IList<IUploadItem> uploadItems)
        {
            var response = new UploadResponse();

            if (string.IsNullOrEmpty(bucketName))
            {
                response.StatusCode = ServiceStatusCode.InvalidBucketName;
                return response;
            }
            if (!await client.DoesS3BucketExistAsync(bucketName))
            {
                response.StatusCode = ServiceStatusCode.BucketDoesNotExist;
                return response;
            }

            using (TransferUtility transferUtility = new TransferUtility(client))
            {
                response.StatusCode = ServiceStatusCode.OK;
                response.UploadedItems = new List<IUploadItem>();
                response.FailedItems = new List<IUploadItemStatus>();
                foreach (var item in uploadItems)
                {
                    if (string.IsNullOrEmpty(item.KeyName))
                    {
                        response.FailedItems.Add(new UploadItemStatus(item, UploadItemStatsCode.InvalidKeyName));
                        continue;
                    }

                    try
                    {
                        var content = item.GetStream();

                        TransferUtilityUploadRequest request = new TransferUtilityUploadRequest
                        {
                            AutoCloseStream = true,
                            BucketName = bucketName,
                            Key = item.KeyName,
                            InputStream = item.GetStream(),
                        };
                        await transferUtility.UploadAsync(request);
                        // client.PutObjectAsync
                        response.UploadedItems.Add(item);
                    }
                    catch (AmazonS3Exception ex)
                    {
                        response.FailedItems.Add(new UploadItemStatus(item, ex));
                    }
                    catch (IOException ex)
                    {
                        response.FailedItems.Add(new UploadItemStatus(item, ex));
                    }
                    catch (Exception ex)
                    {
                        response.FailedItems.Add(new UploadItemStatus(item, ex));   
                    }
                    
                }
            }

            return response;
        }

        /// <summary>
        /// List objects in s3 bucket
        /// </summary>
        /// <param name="bucketName">The bucket name to list</param>
        /// <returns>
        ///     ListResponse contains all file items in the bucket when StatusCode is ServiceStatusCode.OK
        ///     ListResponse contains ServiceStatusCode when InvalidBucketName, InvalidBucketName
        /// </returns>
        public async Task<ListResponse> ListAsync(string bucketName, IEnumerable<string> keyNames)
        {
            if (string.IsNullOrEmpty(bucketName))
            {
                return new ListResponse { StatusCode = ServiceStatusCode.InvalidBucketName };
            }
            if (!await client.DoesS3BucketExistAsync(bucketName))
            {
                return new ListResponse { StatusCode = ServiceStatusCode.InvalidBucketName };
            }

            ListObjectsResponse request = new ListObjectsResponse { Name = bucketName };
            if (keyNames != null && keyNames.Count() > 0)
            {
                request.S3Objects = keyNames.Select(key => new S3Object { Key = key }).ToList();
            }

            var s3Response = await client.ListObjectsAsync(bucketName);

            if (s3Response.HttpStatusCode == HttpStatusCode.OK)
            {
                return new ListResponse()
                {
                    StatusCode = ServiceStatusCode.OK,
                    HttpStatusCode = s3Response.HttpStatusCode,
                    Files = s3Response.S3Objects.Select(s3Object =>
                    {
                        IFileItem item = new FileItem
                        {
                            Key = s3Object.Key,
                            Owner = s3Object.Owner.DisplayName,
                            Size = s3Object.Size,
                        };
                        return item;
                    })
                };
            } else
            {
                return new ListResponse()
                {
                    StatusCode = ServiceStatusCode.HttpStatusCode,
                    HttpStatusCode = s3Response.HttpStatusCode
                };
            }
        }
    }
}
