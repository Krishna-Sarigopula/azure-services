using Azure.Core.Cryptography;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using System.Reflection.Metadata;

namespace BlobStorageApp.Services
{
    public class StorageService
    {
        readonly BlobServiceClient blobServiceClient;
        public StorageService(BlobServiceClient serviceClient)
        {
            blobServiceClient = serviceClient;
        }

        public bool UploadFile(IFormFile formFile, string containerName, string fileName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var metadata = new Dictionary<string, string>();
            metadata.Add("Version", "Version");

            var tags = new Dictionary<string, string>()
                                     {
                                        { "Sealed", "false" },
                                        { "Content", "image" },
                                        { "Date", "2020-04-20" }
                                     };

            containerClient.CreateIfNotExists(PublicAccessType.None, metadata); // setting metadata
            containerClient.GetBlobClient(fileName).SetTags(tags); //setting tags

            BlobContentInfo blob = containerClient.UploadBlob(fileName, formFile.OpenReadStream());

            return blob.VersionId is not null;
        }

        public IEnumerable<String> GetBlobItems()
        {
            var containerClient = blobServiceClient.GetBlobContainers();
            return containerClient.Select(x => x.Name);
        }

        public IEnumerable<String> GetBlobItems(string containerName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            //containerClient.FindBlobsByTags() find by tags
            return containerClient.GetBlobs().Select(x => x.Name);
        }
    }
}
