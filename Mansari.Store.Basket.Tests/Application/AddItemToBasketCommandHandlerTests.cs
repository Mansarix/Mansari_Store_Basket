// Mansari.Store.Basket.Tests/Application/AddItemToBasketCommandHandlerTests.cs
using Mansari.Store.Basket.Application.Basket.Commands;
using Mansari.Store.Basket.Application.Basket.Common.Abstractions;
using Mansari.Store.Basket.Application.Common.Abstractions;
using Mansari.Store.Basket.Application.DTOs;
using Mansari.Store.Basket.Domain.ValueObjects;
using Moq;
using Xunit;

public class AddItemToBasketCommandHandlerTests
{
    private readonly Mock<IBasketRepository> _basketRepositoryMock;
    private readonly Mock<IBasketCacheService> _cacheServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AddItemToBasketCommandHandler _handler;

    public AddItemToBasketCommandHandlerTests()
    {
        _basketRepositoryMock = new Mock<IBasketRepository>();
        _cacheServiceMock = new Mock<IBasketCacheService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new AddItemToBasketCommandHandler(
            _basketRepositoryMock.Object,
            _cacheServiceMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenTotalPriceExceeds50Million()
    {
        // Arrange
        var userId = 1L;
        var existingBasket = Domain.Entities.Basket.Create(userId);

        // اضافه کردن آیتمی که قیمت کل نزدیک به سقف باشد
        existingBasket.AddItem(101, Quantity.Create(10), Money.Create(4_999_000)); // 49,990,000

        var command = new AddItemToBasketCommand(
            userId,
            new AddBasketItemDTO(ProductId: 102, Quantity: 1, UnitPrice: 20_000)); // +20,000 = 50,010,000

        _basketRepositoryMock
            .Setup(x => x.GetActiveBasketByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBasket);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Basket.TotalPriceExceeded");
        result.Error.Message.Should().Contain("مجموع قیمت سبد از سقف مجاز بیشتر شده است");
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenQuantityExceeds10_ByAddingSameProductTwice()
    {
        // Arrange
        var userId = 1L;
        var productId = 101;
        var existingBasket = Domain.Entities.Basket.Create(userId);

        // ابتدا 8 عدد اضافه شده
        existingBasket.AddItem(productId, Quantity.Create(8), Money.Create(5_000));

        // حالا می‌خواهیم 3 عدد دیگر اضافه کنیم (مجموع = 11)
        var command = new AddItemToBasketCommand(
            userId,
            new AddBasketItemDTO(ProductId: productId, Quantity: 3, UnitPrice: 5_000));

        _basketRepositoryMock
            .Setup(x => x.GetActiveBasketByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBasket);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Basket.InvalidQuantity");
        result.Error.Message.Should().Contain("تعداد هر کالا باید بین 1 تا 10 باشد");
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenBasketIsExpired()
    {
        // Arrange
        var userId = 1L;
        var expiredBasket = Domain.Entities.Basket.Create(userId);

        // منقضی کردن سبد
        expiredBasket.Expire();

        var command = new AddItemToBasketCommand(
            userId,
            new AddBasketItemDTO(ProductId: 101, Quantity: 2, UnitPrice: 5_000));

        _basketRepositoryMock
            .Setup(x => x.GetActiveBasketByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expiredBasket);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Basket.BasketExpired");
        result.Error.Message.Should().Contain("این سبد خرید منقضی شده و قابل تغییر نیست");
    }
}
