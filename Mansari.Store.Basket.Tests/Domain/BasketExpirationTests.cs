using FluentAssertions;
using Mansari.Store.Basket.Domain.Enums;
using Mansari.Store.Basket.Domain.ValueObjects;

namespace Mansari.Store.Basket.Tests.Domain;

// Mansari.Store.Basket.Tests/Domain/BasketTests.cs (اضافه به فایل موجود)
public class BasketExpirationTests
{
    [Fact]
    public void Expire_ShouldChangeStatusToExpired_AndRaiseDomainEvent()
    {
        // Arrange
        var basket = Basket.Domain.Aggregates.Basket.Create(1);
        basket.AddItem(101, Quantity.Create(2), Money.Create(5_000));

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
        var basket = basket.Domain.Aggregation.Basket.Create(userId: 1);
        basket.Expire();

        // Act
        var result = basket.AddItem(101, Quantity.Create(2), Money.Create(5_000));

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Basket.BasketExpired");
    }
}
