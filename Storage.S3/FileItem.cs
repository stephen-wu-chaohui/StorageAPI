using System;
using Storage.Core;

namespace Storage.S3
{
    public class FileItem : IFileItem
    {
        public string Key { get; set; }

        public string Owner { get; set; }

        public long Size { get; set; }

        public string ResourceLocation { get; set; }
    }
}
