# ShoppingCart.Caching

A reusable .NET caching module supporting **Redis** and **distributed in-memory caching** through `IDistributedCache`.

## Features

- Configurable cache provider: `Memory` or `Redis`
- Generic get, set, remove, and existence operations
- Configurable default expiration
- Versioned cache keys and group invalidation
- Dependency injection through `AddCaching()`

## Setup

Add a project reference to `ShoppingCart.Caching` from your consuming project.

Register the module in `Program.cs`:

```
builder.Services.AddCaching(builder.Configuration);
```

## Configuration

Configure `appsettings.json`:

```
{
  "Cache": {
    "Provider": "Memory",
    "DefaultExpiration": "00:10:00",
    "InstanceName": "MyProject:"
  },
  "Redis": {
    "ConnectionString": ""
  }
}
```

Set `Cache:Provider` to `Redis` and provide `Redis:ConnectionString` to use Redis.

## Usage

Inject `ICacheService` into your application services:

```
await cache.SetAsync("products:all", products);

var products = await cache.GetAsync<List<ProductDto>>(
    "products:all");

await cache.RemoveAsync("products:all");
```

Use versioned keys and `InvalidateCacheByVersionAsync()` to invalidate groups of cached entries.

## Notes

- Memory caching is local to the application process.
- Redis supports sharing cached data across application instances.
- Cache entries expire according to the configured or explicitly supplied expiration.
- Version-based invalidation makes old entries inaccessible through new versioned keys; it does not immediately delete them.
- Store Redis credentials securely outside source control.