using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Google;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Cloud.Storage.V1;
using ImageServer.Core.Model;
using MongoDB.Bson;

namespace ImageServer.Core.Services.FileAccess
{
    public class GoogleStorageBucketAccess : IFileAccessStrategy
    {
        public async Task<byte[]> GetFileAsync(HostConfig host, string file)
        {
            try
            {
                await using var stream = new MemoryStream();
                // Instantiates a client.
                var storage = StorageClient.Create();

                // The name for the new bucket.
                string bucketName = host.Backend;

                await storage.DownloadObjectAsync(bucketName, file, stream);

                return stream.GetBuffer();
            }
            catch (TokenResponseException ex)
            {
                throw new UnauthorizedAccessException($"Google Storage Bucket ({host.Slug}|{host.Backend}): {ex.Message}");
            }
            catch (GoogleApiException ex)
            {
                if (ex.HttpStatusCode == HttpStatusCode.NotFound)
                {
                    throw new FileNotFoundException(ex.Message, file);
                }

                throw;
            }
        }

        public Task<ObjectId> PostFileAsync(HostConfig hostConfig, byte[] bytes) => throw new NotImplementedException();
    }
}