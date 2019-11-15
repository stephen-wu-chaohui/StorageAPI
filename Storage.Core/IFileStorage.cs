using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Storage.Core
{
    public interface IFileStorage
    {
        Task<UploadResponse> UploadAsync(string bucketName, IList<IFormFile> formFiles);
        Task<ListResponse> ListAsync(string bucketName);
    }
}
