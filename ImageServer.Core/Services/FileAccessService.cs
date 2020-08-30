using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImageServer.Core.Model;
using ImageServer.Core.Services.FileAccess;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

namespace ImageServer.Core.Services
{
    public class FileAccessService : IFileAccessService
    {
        private readonly List<HostConfig> _hosts;
        private readonly IDictionary<HostType, IFileAccessStrategy> _strategies;

        public FileAccessService(IOptions<List<HostConfig>> hosts, IDictionary<HostType, IFileAccessStrategy> strategies)
        {
            _hosts = hosts?.Value ?? throw new ArgumentNullException(nameof(hosts));
            _strategies = strategies ?? throw new ArgumentNullException(nameof(strategies));
        }

        public HostConfig GetHostConfig(string slug)
        {
            HostConfig host = _hosts.Find(x => x.Slug == slug);

            if (host == null)
            {
                throw new SlugNotFoundException($"{slug}");
            }

            return host;
        }

        public async Task<byte[]> GetFileAsync(string slug, string file)
        {
            HostConfig host = _hosts.Find(x => x.Slug == slug);
            if (host == null)
            {
                throw new SlugNotFoundException($"{slug}");
            }

            IFileAccessStrategy access = _strategies[host.Type];

            var fileBytes = await access.GetFileAsync(host, file);

            return fileBytes;
        }

        public Task<ObjectId> PostFileAsync(string slug, byte[] data)
        {
            HostConfig host = _hosts.Find(x => x.Slug == slug);

            if (host == null)
            {
                throw new SlugNotFoundException($"{slug}");
            }

            IFileAccessStrategy access = _strategies[host.Type];

            return access.PostFileAsync(host, data);
        }
    }
}