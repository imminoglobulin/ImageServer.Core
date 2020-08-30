using System.Threading.Tasks;
using ImageServer.Core.Model;
using MongoDB.Bson;

namespace ImageServer.Core.Services
{
    public interface IFileAccessService
    {
        HostConfig GetHostConfig(string slug);

        Task<byte[]> GetFileAsync(string slug, string file);

        Task<ObjectId> PostFileAsync(string slug, byte[] array);
    }
}