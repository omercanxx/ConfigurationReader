using ConfigurationReader.Application.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace ConfigurationReader.Application.Services
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache distributedCache;

        public RedisCacheService(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var cachedData = await this.distributedCache.GetAsync(key);

            if (cachedData == null)
                return default;

            var json = Encoding.UTF8.GetString(cachedData);

            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var json = JsonConvert.SerializeObject(value);
            var bytes = Encoding.UTF8.GetBytes(json);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromHours(1)
            };

            await this.distributedCache.SetAsync(key, bytes, options);
        }

        public async Task RemoveAsync(string key)
        {
            await this.distributedCache.RemoveAsync(key);
        }
    }
}