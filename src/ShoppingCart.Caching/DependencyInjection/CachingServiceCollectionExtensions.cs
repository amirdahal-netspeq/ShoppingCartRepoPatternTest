using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShoppingCart.Caching.Abstractions;
using ShoppingCart.Caching.Configuration;
using ShoppingCart.Caching.Services;

namespace ShoppingCart.Caching.DependencyInjection
{
    public static class CachingServiceCollectionExtensions
    {
        public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

            var cacheSection = configuration.GetSection("Cache");
            var options = new CacheOptions();

            cacheSection.Bind(options);

            options.RedisConnectionString = configuration["Redis:ConnectionString"];

            if (options.DefaultExpiration <= TimeSpan.Zero)
            {
                throw new InvalidOperationException("Cache:DefaultExpiration must be greater than zero.");
            }

            services.Configure<CacheOptions>(configured =>
            {
                cacheSection.Bind(configured);
                configured.RedisConnectionString = configuration["Redis:ConnectionString"];
            });

            if (options.Provider.Equals("Redis", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(options.RedisConnectionString))
                {
                    throw new InvalidOperationException(
                    "Redis:ConnectionString is required " +
                    "when Cache:Provider is Redis.");

                }

                services.AddStackExchangeRedisCache(redis =>
                {
                    redis.Configuration = options.RedisConnectionString;
                    redis.InstanceName = options.InstanceName;
                });
            }
            else if (options.Provider.Equals("Memory", StringComparison.OrdinalIgnoreCase))
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                throw new InvalidOperationException($"Unsupported cache provider: {options.Provider}");
            }

            services.AddScoped<ICacheService, CacheService>();

            return services;
        }
    }
}
