using System.Threading.Tasks;
using ImageServer.Core.Model;
using MongoDB.Bson;

namespace ImageServer.Core.Services.FileAccess
{
    public interface IFileAccessStrategy
    {
        Task<byte[]> GetFileAsync(HostConfig host, string file);

        Task<ObjectId> PostFileAsync(HostConfig hostConfig, byte[] bytes);
    }
}