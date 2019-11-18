using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storage.Core
{
    public interface ICloudStorage
    {
        Task<UploadResponse> UploadAsync(string bucketName, IList<IUploadItem> uploadItems);
        Task<ListResponse> ListAsync(string bucketName, IEnumerable<string> keyNames = null);
    }
}
