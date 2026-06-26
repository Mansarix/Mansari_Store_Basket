using DotNet.Testcontainers.Builders;
using global::Mansari.Store.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Testcontainers.MsSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;
using Xunit;

namespace Mansari.Store.Basket.Tests.Integration;

public class BasketControllerIntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _sqlContainer;
    private readonly RedisContainer _redisContainer;
    private readonly RabbitMqContainer _rabbitMqContainer;

    public BasketControllerIntegrationTestFactory()
    {
        // SQL Server Container
        _sqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("Test@Password123")
            .WithPortBinding(1433, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
            .Build();

        // Redis Container
        _redisContainer = new RedisBuilder()
            .WithImage("redis:7-alpine")
            .WithPortBinding(6379, true)
            .Build();

        // RabbitMQ Container
        _rabbitMqContainer = new RabbitMqBuilder()
            .WithImage("rabbitmq:3-management-alpine")
            .WithPortBinding(5672, true)
            .WithPortBinding(15672, true)
            .WithUsername("guest")
            .WithPassword("guest")
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // حذف DbContext موجود
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
            services.RemoveAll<AppDbContext>();

            // اضافه کردن DbContext با connection string از TestContainer
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(_sqlContainer.GetConnectionString());
            });

            // تنظیم Redis
            services.RemoveAll(typeof(StackExchange.Redis.IConnectionMultiplexer));
            var redisConnection = StackExchange.Redis.ConnectionMultiplexer.Connect(_redisContainer.GetConnectionString());
            services.AddSingleton<StackExchange.Redis.IConnectionMultiplexer>(redisConnection);

            // تنظیم RabbitMQ
            var rabbitConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["ConnectionStrings:RabbitMQ"] = $"amqp://guest:guest@{_rabbitMqContainer.Hostname}:{_rabbitMqContainer.GetMappedPublicPort(5672)}"
                }!)
                .Build();

            services.AddSingleton<IConfiguration>(rabbitConfig);
        });

        builder.UseEnvironment("Testing");
    }

    public async Task InitializeAsync()
    {
        // شروع کانتینرها
        await _sqlContainer.StartAsync();
        await _redisContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();

        // اجرای Migrations
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _sqlContainer.DisposeAsync();
        await _redisContainer.DisposeAsync();
        await _rabbitMqContainer.DisposeAsync();
    }

    public AppDbContext GetDbContext()
    {
        var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    public StackExchange.Redis.IConnectionMultiplexer GetRedisConnection()
    {
        return Services.GetRequiredService<StackExchange.Redis.IConnectionMultiplexer>();
    }
}
