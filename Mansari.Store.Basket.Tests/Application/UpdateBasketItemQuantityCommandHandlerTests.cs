// Mansari.Store.Basket.Tests/Application/UpdateBasketItemQuantityCommandHandlerTests.cs
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
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateQuantity_Successfully()
    {
        // Arrange
        var userId = 1L;
        var productId = 101;
        var basket = new  Mansari.Store.Basket.Domain.Aggregates.Basket(userId);
        basket.AddItem(productId, Quantity.Create(3),new Money(10_000));

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
        var basket = Domain.Entities.Basket.Create(userId);
        basket.AddItem(productId, Quantity.Create(5), Money.Create(10_000));

        var command = new UpdateBasketItemQuantityCommand(userId, productId, 11); // بیشتر از سقف

        _basketRepositoryMock
            .Setup(x => x.GetActiveBasketByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(basket);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Basket.InvalidQuantity");
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenItemNotFound()
    {
        // Arrange
        var userId = 1L;
        var basket = Domain.Entities.Basket.Create(userId);
        basket.AddItem(101, Quantity.Create(2), Money.Create(10_000));

        var command = new UpdateBasketItemQuantityCommand(userId, productId: 999, newQuantity: 3);

        _basketRepositoryMock
            .Setup(x => x.GetActiveBasketByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(basket);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Basket.ItemNotFound");
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenBasketIsExpired()
    {
        // Arrange
        var userId = 1L;
        var basket = Domain.Entities.Basket.Create(userId);
        basket.AddItem(101, Quantity.Create(3), Money.Create(10_000));
        basket.Expire(); // منقضی شدن

        var command = new UpdateBasketItemQuantityCommand(userId, productId: 101, newQuantity: 5);

        _basketRepositoryMock
            .Setup(x => x.GetActiveBasketByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(basket);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Basket.BasketExpired");
    }
}
