using FluentAssertions;
using Mansari.Store.Basket.Domain.Enums;
using Xunit;

namespace Mansari.Store.Basket.Tests.Domain;

public class BasketExpirationTests
{
    [Fact]
    public void Expire_ShouldChangeStatusToExpired_AndRaiseDomainEvent()
    {
        // Arrange
        var basket = new Basket.Domain.Aggregates.Basket(1);
        basket.AddItem(101, 2, 5000);

        // Act
        basket.Expire();

        // Assert
        basket.Status.Should().Be(BasketStatus.Expired);
        basket.DomainEvents.Should().ContainSingle(e => e is BasketExpiredEvent);

        var expiredEvent = basket.DomainEvents.OfType<BasketExpiredEvent>().First();
        expiredEvent.BasketId.Should().Be(basket.Id);
        expiredEvent.UserId.Should().Be(1);
    }

    [Fact]
    public void AddItem_ShouldFail_WhenBasketIsExpired()
    {
        // Arrange
        var basket = new Mansari.Store.Basket.Domain.Aggregates.Basket(1);
        basket.Expire();

        // Act
        basket.AddItem(101, 2, 5_000);

        // Assert
    }
}
