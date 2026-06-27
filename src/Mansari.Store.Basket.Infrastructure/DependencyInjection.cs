using Mansari.Store.Basket.Application.Basket.Common.Abstractions;
using Mansari.Store.Basket.Application.Basket.Common.Dispatchers;
using Mansari.Store.Basket.Application.Common.Abstractions;
using Mansari.Store.Basket.Infrastructure.Messaging;
using Mansari.Store.Basket.Infrastructure.Persistence;
using Mansari.Store.Basket.Infrastructure.Services.Redis;
using Mansari.Store.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Mansari.Store.Basket.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("SqlServer"));
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IBasketRepository, BasketRepository>();

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")));

        services.AddSingleton<IBasketCacheService, BasketCacheService>();

        services.AddHostedService<BackgroundServices.BasketExpirationBackgroundService>();


        services.AddSingleton<IBasketEventPublisher>(sp =>
        {
            var connectionString = configuration.GetConnectionString("RabbitMQ")
                ?? "amqp://guest:guest@localhost:5672";
            return new MessageBus.RabbitMqEventPublisher(connectionString);
        });


        return services;
    }
}
