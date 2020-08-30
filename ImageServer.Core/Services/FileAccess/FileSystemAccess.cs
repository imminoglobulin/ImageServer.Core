using System;
using System.IO;
using System.Threading.Tasks;
using ImageServer.Core.Model;
using MongoDB.Bson;

namespace ImageServer.Core.Services.FileAccess
{
    public class FileSystemAccess : IFileAccessStrategy
    {
        public async Task<byte[]> GetFileAsync(HostConfig host, string file)
        {
            try
            {
                string filepath = host.Path + file;
                var b = await File.ReadAllBytesAsync(filepath);
                return b;
            }
            catch (FileNotFoundException)
            {
                if (host.FallbackImage == null)
                {
                    throw;
                }

                if (file == host.FallbackImage)
                {
                    throw new FileNotFoundException("Fallback image not found");
                }

                throw new RedirectToFallbackException(host.FallbackImage, "Image not found, redirect to fallback");
            }
        }

        public Task<ObjectId> PostFileAsync(HostConfig hostConfig, byte[] bytes) => throw new NotImplementedException();
    }
}