using FluentAssertions;
using Mansari.Store.Basket.Application.Basket.Commands;
using Mansari.Store.Basket.Application.Basket.Common.Abstractions;
using Mansari.Store.Basket.Application.Common.Abstractions;
using Mansari.Store.Basket.Domain.ValueObjects;
using Moq;
using Xunit;

public class UpdateBasketItemQuantityCommandHandlerTests
{
    private readonly Mock<IBasketRepository> _basketRepositoryMock;
    private readonly Mock<IBasketCacheService> _cacheServiceMock;
    private readonly IBasketEventPublisher _eventPublisher;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdateBasketItemQuantityCommandHandler _handler;

    public UpdateBasketItemQuantityCommandHandlerTests()
    {
        _basketRepositoryMock = new Mock<IBasketRepository>();
        _cacheServiceMock = new Mock<IBasketCacheService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new UpdateBasketItemQuantityCommandHandler(
            _basketRepositoryMock.Object,
            _cacheServiceMock.Object,
            _eventPublisher);
    }

    [Fact]
    public async Task Handle_ShouldUpdateQuantity_Successfully()
    {
        // Arrange
        var userId = 1L;
        var productId = 101;
        var basket = new  Mansari.Store.Basket.Domain.Aggregates.Basket(userId);
        basket.AddItem(productId,  3,new Money(10_000));

        var command = new UpdateBasketItemQuantityCommand(userId, productId, 5);

        _basketRepositoryMock
            .Setup(x => x.GetActiveBasketByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(basket);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        basket.Items.First(x => x.ProductId == productId).Quantity.Value.Should().Be(5);
        _cacheServiceMock.Verify(x => x.RemoveAsync($"basket:{userId}", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenNewQuantityExceeds10()
    {
        // Arrange
        var userId = 1L;
        var productId = 101;
        var basket = new Mansari.Store.Basket.Domain.Aggregates.Basket(userId);
        basket.AddItem(productId,  5, 10000);

        var command = new UpdateBasketItemQuantityCommand(userId, productId, 11); // بیشتر از سقف

        _basketRepositoryMock
            .Setup(x => x.GetActiveBasketByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(basket);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenItemNotFound()
    {
        // Arrange
        var userId = 1L;
        var basket = new Mansari.Store.Basket.Domain.Aggregates.Basket(userId);
        basket.AddItem(101,  2, 10_000);

        var command = new UpdateBasketItemQuantityCommand(userId, 999, 3);

        _basketRepositoryMock
            .Setup(x => x.GetActiveBasketByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(basket);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenBasketIsExpired()
    {
        // Arrange
        var userId = 1L;
        var basket = new Mansari.Store.Basket.Domain.Aggregates.Basket(userId);
        basket.AddItem(101,  3, 10000);
        basket.Expire(); // منقضی شدن

        var command = new UpdateBasketItemQuantityCommand(userId, 101, 5);

        _basketRepositoryMock
            .Setup(x => x.GetActiveBasketByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(basket);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }
}
