using System.IO;

namespace Storage.Core
{
    public interface IUploadItem
    {
        public string KeyName { get; }
        public Stream GetStream();
    }
}
