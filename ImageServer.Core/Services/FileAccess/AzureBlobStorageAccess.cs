using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using ImageServer.Core.Model;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;

namespace ImageServer.Core.Services.FileAccess
{
    public class AzureBlobStorageAccess : IFileAccessStrategy
    {
        public AzureBlobStorageAccess(IConfiguration configuration)
        {
            ConnectionString = configuration.GetValue<string>("AzureConnectionString");
        }

        public string ConnectionString { get; }

        public async Task<byte[]> GetFileAsync(HostConfig host, string file)
        {
            var container = new AzureBlobContainer(host, file);

            var serviceClient = new BlobServiceClient(ConnectionString);
            BlobContainerClient containerClient = serviceClient.GetBlobContainerClient(container.ContainerClientName);
            if (!await containerClient.ExistsAsync())
            {
                throw new FileNotFoundException(file);
            }

            BlobClient blobClient = containerClient.GetBlobClient(container.BlobClientName);
            if (!await blobClient.ExistsAsync())
            {
                throw new FileNotFoundException(file);
            }

            await using var stream = new MemoryStream();
            var download = await blobClient.DownloadAsync();
            await download.Value.Content.CopyToAsync(stream);
            return stream.GetBuffer();
        }

        public Task<ObjectId> PostFileAsync(HostConfig hostConfig, byte[] bytes) => throw new NotImplementedException();
    }
}