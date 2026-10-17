namespace ShoppingCart.Caching.Configuration
{
    public sealed class CacheOptions
    {
        public string Provider { get; set; } = "Memory";
        public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(10);
        public string? RedisConnectionString { get; set; }
        public string? InstanceName { get; set; }
    }
}
