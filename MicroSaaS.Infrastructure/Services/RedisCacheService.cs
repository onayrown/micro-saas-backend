using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using MicroSaaS.Application.Interfaces.Services;
using System.Text;

namespace MicroSaaS.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly DistributedCacheEntryOptions _defaultOptions;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
        _defaultOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        };
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var bytes = await _cache.GetAsync(key);
        if (bytes == null) return default;
        
        var json = Encoding.UTF8.GetString(bytes);
        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var options = expiration.HasValue
            ? new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration }
            : _defaultOptions;

        var serializedValue = JsonSerializer.Serialize(value);
        var bytes = Encoding.UTF8.GetBytes(serializedValue);
        await _cache.SetAsync(key, bytes, options);
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        var value = await _cache.GetAsync(key);
        return value != null;
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        var cachedValue = await GetAsync<T>(key);
        if (cachedValue != null)
        {
            return cachedValue;
        }

        var value = await factory();
        await SetAsync(key, value, expiration);
        return value;
    }
} 