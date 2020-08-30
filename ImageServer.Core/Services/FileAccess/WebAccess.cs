using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ImageServer.Core.Model;
using MongoDB.Bson;

namespace ImageServer.Core.Services.FileAccess
{
    public class WebAccess : IFileAccessStrategy
    {
        public async Task<byte[]> GetFileAsync(HostConfig host, string file)
        {
            var url = $"{host.Backend}/{file}";

            await using var stream = new MemoryStream();
            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            using HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new FileNotFoundException("File not found", url);
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpRequestException($"Http request not OK: {response.StatusCode}, url: {url}");
            }

            await using Stream contentStream = await response.Content.ReadAsStreamAsync();

            await contentStream.CopyToAsync(stream);

            return stream.TryGetBuffer(out var data) ? data.Array : null;
        }

        public Task<ObjectId> PostFileAsync(HostConfig hostConfig, byte[] bytes) => throw new NotImplementedException();
    }
}