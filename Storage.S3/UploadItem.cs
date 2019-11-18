using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Storage.Core;

namespace Storage.S3
{
    public class UploadItem : IUploadItem
    {
        private IFormFile FormFile { get; }

        public UploadItem(IFormFile formFile)
        {
            FormFile = formFile;
        }

        public string KeyName => FormFile.FileName;

        public Stream GetStream() => FormFile.OpenReadStream();
    }
}
