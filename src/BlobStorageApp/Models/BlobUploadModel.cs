using Azure.Storage.Blobs.Models;

namespace BlobStorageApp.Models
{
    public class BlobUploadModel
    {
        public string Name { get; set; }

        public IFormFile FormFile { get; set; }

        public string FileName { get; set; }
    }
}
