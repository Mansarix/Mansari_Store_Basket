using Mansari.Store.Basket.Application.Basket.Common.Abstractions;
using StackExchange.Redis;
using System.Text.Json;

namespace Mansari.Store.Basket.Infrastructure.Services.Redis;


public sealed class BasketCacheService(IConnectionMultiplexer redis)
    : IBasketCacheService
{
    private readonly IDatabase _database = redis.GetDatabase();

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var value = await _database.StringGetAsync(key);

        if (value.IsNullOrEmpty)
            return default;

        return JsonSerializer.Deserialize<T>(value!);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(value);

        await _database.StringSetAsync(key, json, expiration);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _database.KeyDeleteAsync(key);
    }
}
