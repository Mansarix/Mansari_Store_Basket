using FluentAssertions;
using Mansari.Store.Basket.Domain.Events;
using Mansari.Store.Basket.Infrastructure.Persistence;
using Mansari.Store.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Mansari.Store.Basket.Tests.Integration;

public class RabbitMqIntegrationTests : IClassFixture<BasketControllerIntegrationTestFactory>
{
    private readonly BasketControllerIntegrationTestFactory _factory;

    public RabbitMqIntegrationTests(BasketControllerIntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ExpireBasket_ShouldPublishBasketExpiredEvent_ToRabbitMq()
    {
        // Arrange
        var userId = 200L;
        var receivedEvents = new List<BasketExpiredEvent>();
        var eventReceived = new TaskCompletionSource<bool>();

        // ایجاد Consumer برای دریافت رویداد
        var connectionFactory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "guest",
            Password = "guest"
        };

        using var connection = connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare("basket-expired-events", durable: true, exclusive: false, autoDelete: false);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var basketEvent = JsonSerializer.Deserialize<BasketExpiredEvent>(message);

            if (basketEvent != null)
            {
                receivedEvents.Add(basketEvent);
                eventReceived.TrySetResult(true);
            }
        };

        channel.BasicConsume("basket-expired-events", autoAck: true, consumer);

        // ایجاد و منقضی کردن سبد
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var basket = new Mansari.Store.Basket.Domain.Aggregates.Basket(userId);
        basket.AddItem(20001, Mansari.Store.Basket.Domain.ValueObjects.Quantity.Create(2), new Mansari.Store.Basket.Domain.ValueObjects.Money(50_000));

        dbContext.Baskets.Add(basket);
        await dbContext.SaveChangesAsync();

        basket.Expire();
        await dbContext.SaveChangesAsync();

        // Act - صبر برای دریافت رویداد
        var received = await Task.WhenAny(eventReceived.Task, Task.Delay(5000)) == eventReceived.Task;

        // Assert
        received.Should().BeTrue("باید رویداد BasketExpired منتشر شده باشد");
        receivedEvents.Should().HaveCount(1);
        receivedEvents.First().UserId.Should().Be(userId);
        receivedEvents.First().BasketId.Should().Be(basket.Id);
    }
}
