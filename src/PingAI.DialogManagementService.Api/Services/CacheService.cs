using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using PingAI.DialogManagementService.Application.Interfaces.Configuration;
using PingAI.DialogManagementService.Application.Interfaces.Services.Caching;
using PingAI.DialogManagementService.Infrastructure.Utils;

namespace PingAI.DialogManagementService.Api.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IAppConfig _appConfig;

        public CacheService(IDistributedCache distributedCache, IAppConfig appConfig)
        {
            _distributedCache = distributedCache;
            _appConfig = appConfig;
        }

        public Task SetObject<T>(string key, T value) where T : class, new()
        {
            var json = JsonUtils.Serialize(value);
            return _distributedCache.SetAsync(PrependGlobalKeyPrefix(key), System.Text.Encoding.UTF8.GetBytes(json));
        }

        public async Task<T?> GetObject<T>(string key, T? fallbackValue = default) where T : class, new()
        {
            var data = await _distributedCache.GetAsync(PrependGlobalKeyPrefix(key));
            if (data == null)
                return fallbackValue;
            var json = System.Text.Encoding.UTF8.GetString(data);
            return JsonUtils.Deserialize<T>(json);
        }

        public Task SetString(string key, string value)
        {
            return _distributedCache.SetStringAsync(PrependGlobalKeyPrefix(key), value);
        }

        public async Task<string?> GetString(string key, string? fallbackValue = null)
        {
            var value = await _distributedCache.GetStringAsync(PrependGlobalKeyPrefix(key));
            return value ?? fallbackValue;
        }

        private string PrependGlobalKeyPrefix(string key) => $"{_appConfig.GlobalCachePrefix}{key}";
    }
}
