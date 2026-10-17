namespace ShoppingCart.Caching.Abstractions
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key, CancellationToken ct = default);

        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default);

        Task RemoveAsync(string key, CancellationToken ct = default);

        Task<bool> ExistsAsync(string key, CancellationToken ct = default);

        Task InvalidateCacheByVersionAsync(string versionKeyPrefix, CancellationToken ct = default);

        Task<string> GetCacheVersionAsync(string versionKeyPrefix, CancellationToken ct = default);

        Task<string> GetVersionedCacheKeyAsync(string baseKey, string versionKeyPrefix, CancellationToken ct = default);
    }
}
