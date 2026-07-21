using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using PollBuilder.Application.Interfaces.Services;

namespace PollBuilder.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await _cache.GetStringAsync(key);

        if (string.IsNullOrEmpty(json))
            return default;

        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow =
                expiration ?? TimeSpan.FromMinutes(5)
        };

        var json = JsonSerializer.Serialize(value);

        await _cache.SetStringAsync(
            key,
            json,
            options);
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }
}