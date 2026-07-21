using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using PollBuilder.Application.Interfaces.Services;

namespace PollBuilder.Infrastructure.Catching;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(
        IDistributedCache cache,
        ILogger<RedisCacheService> logger
    )
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(
        string key
    )
    {
        try
        {
            var json =
                await _cache.GetStringAsync(key);

            if (string.IsNullOrWhiteSpace(json))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(
                json
            );
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Unable to read cache key {CacheKey}. " +
                "The request will continue without cache.",
                key
            );

            return default;
        }
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null
    )
    {
        try
        {
            var options =
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow =
                        expiration
                        ?? TimeSpan.FromMinutes(5)
                };

            var json =
                JsonSerializer.Serialize(value);

            await _cache.SetStringAsync(
                key,
                json,
                options
            );
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Unable to write cache key {CacheKey}. " +
                "The request will continue without cache.",
                key
            );
        }
    }

    public async Task RemoveAsync(
        string key
    )
    {
        try
        {
            await _cache.RemoveAsync(key);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Unable to remove cache key {CacheKey}.",
                key
            );
        }
    }
}