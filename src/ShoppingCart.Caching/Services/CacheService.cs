using Microsoft.Extensions.Caching.Distributed;
using ShoppingCart.Caching.Abstractions;
using ShoppingCart.Caching.Configuration;
using System.Text.Json;

namespace ShoppingCart.Caching.Services
{
    internal sealed class CacheService : ICacheService
    {
        private const string VersionKeyPrefix = "__cache_version:";

        private readonly IDistributedCache _cache;
        private readonly CacheOptions _options;


        public CacheService(IDistributedCache cache, Microsoft.Extensions.Options.IOptions<CacheOptions> options)
        {
            _cache = cache;
            _options = options.Value;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);

            var json = await _cache.GetStringAsync(key, ct);

            if (json is null)
                return default;

            return JsonSerializer.Deserialize<T>(json);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);

            var json = JsonSerializer.Serialize(value);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? _options.DefaultExpiration
            };

            await _cache.SetStringAsync(key, json, options, ct);
        }

        public Task RemoveAsync(string key, CancellationToken ct = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);

            return _cache.RemoveAsync(key, ct);
        }

        public async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);

            var value = await _cache.GetAsync(key, ct);

            return value is not null;
        }

        public async Task<string> GetCacheVersionAsync(string versionKeyPrefix, CancellationToken ct = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(versionKeyPrefix);

            var version = await _cache.GetStringAsync(VersionKeyPrefix + versionKeyPrefix, ct);

            return version ?? "0";
        }

        public async Task<string> GetVersionedCacheKeyAsync(string baseKey, string versionKeyPrefix, CancellationToken ct = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(baseKey);
            ArgumentException.ThrowIfNullOrWhiteSpace(versionKeyPrefix);

            var version = await GetCacheVersionAsync(versionKeyPrefix, ct);

            return $"{baseKey}:v{version}";
        }

        public async Task InvalidateCacheByVersionAsync(string versionKeyPrefix, CancellationToken ct = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(versionKeyPrefix);

            var currentVersion = await GetCacheVersionAsync(versionKeyPrefix, ct);

            var newVersion = Guid.NewGuid().ToString("N");

            await _cache.SetStringAsync(VersionKeyPrefix + versionKeyPrefix, newVersion, ct);
        }
    }
}
