using FluentAssertions;
using Mansari.Store.Basket.API.Common.Results;
using Mansari.Store.Basket.API.Contracts;
using Mansari.Store.Basket.Application.DTOs;
using Mansari.Store.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Mansari.Store.Basket.Tests.Integration;

public class BasketControllerIntegrationTests : IClassFixture<BasketControllerIntegrationTestFactory>
{
    private readonly HttpClient _client;
    private readonly BasketControllerIntegrationTestFactory _factory;

    public BasketControllerIntegrationTests(BasketControllerIntegrationTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetOrCreateBasket_ShouldReturnNewBasket_WhenCalledFirstTime()
    {
        // Arrange
        var userId = 100L;

        // Act
        var response = await _client.GetAsync($"/api/v1/basket/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResult<BasketDTO>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.UserId.Should().Be(userId);
        result.Data.Items.Should().BeEmpty();
        result.Data.Status.Should().Be("Active");
    }

    [Fact]
    public async Task AddItemToBasket_ShouldAddSuccessfully()
    {
        // Arrange
        var userId = 101L;
        var addRequest = new AddBasketItemDTO(ProductId: 1001, Quantity: 3, UnitPrice: 50_000);

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/basket/{userId}/items", addRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResult<BasketDTO>>();
        result!.Success.Should().BeTrue();
        result.Data!.Items.Should().HaveCount(1);
        result.Data.Items.First().ProductId.Should().Be(1001);
        result.Data.Items.First().Quantity.Should().Be(3);
        result.Data.TotalPrice.Should().Be(150_000);
    }

    [Fact]
    public async Task AddItemToBasket_ShouldFail_WhenTotalPriceExceeds50Million()
    {
        // Arrange
        var userId = 102L;

        // اضافه کردن آیتم اول (49,500,000)
        await _client.PostAsJsonAsync($"/api/v1/basket/{userId}/items",
            new AddBasketItemDTO(ProductId: 2001, Quantity: 10, UnitPrice: 4_950_000));

        // تلاش برای اضافه کردن آیتم دوم که کل قیمت از سقف عبور کند
        var secondItem = new AddBasketItemDTO(ProductId: 2002, Quantity: 2, UnitPrice: 300_000);

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/basket/{userId}/items", secondItem);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Content.ReadFromJsonAsync<ApiResult<BasketDTO>>();
        result!.Success.Should().BeFalse();
        result.Errors.Should().NotBeNull();
    }

    [Fact]
    public async Task AddItemToBasket_ShouldAccumulateQuantity_WhenSameProductAddedTwice()
    {
        // Arrange
        var userId = 103L;
        var productId = 3001;

        // اضافه کردن 5 عدد
        await _client.PostAsJsonAsync($"/api/v1/basket/{userId}/items",
            new AddBasketItemDTO(ProductId: productId, Quantity: 5, UnitPrice: 10_000));

        // اضافه کردن 3 عدد دیگر
        var secondAdd = new AddBasketItemDTO(ProductId: productId, Quantity: 3, UnitPrice: 10_000);

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/basket/{userId}/items", secondAdd);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResult<BasketDTO>>();
        result!.Data!.Items.Should().HaveCount(1);
        result.Data.Items.First().Quantity.Should().Be(8); // 5 + 3
    }

    [Fact]
    public async Task AddItemToBasket_ShouldFail_WhenAccumulatedQuantityExceeds10()
    {
        // Arrange
        var userId = 104L;
        var productId = 4001;

        // اضافه کردن 8 عدد
        await _client.PostAsJsonAsync($"/api/v1/basket/{userId}/items",
            new AddBasketItemDTO(ProductId: productId, Quantity: 8, UnitPrice: 10_000));

        // تلاش برای اضافه کردن 3 عدد دیگر (مجموع = 11)
        var secondAdd = new AddBasketItemDTO(ProductId: productId, Quantity: 3, UnitPrice: 10_000);

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/basket/{userId}/items", secondAdd);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Content.ReadFromJsonAsync<ApiResult<BasketDTO>>();
        result!.Success.Should().BeFalse();
        result.Errors.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateItemQuantity_ShouldUpdateSuccessfully()
    {
        // Arrange
        var userId = 105L;
        var productId = 5001;

        // افزودن آیتم
        await _client.PostAsJsonAsync($"/api/v1/basket/{userId}/items",
            new AddBasketItemDTO(ProductId: productId, Quantity: 3, UnitPrice: 20_000));

        var updateRequest = new UpdateBasketItemQuantityRequest(7);

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/basket/{userId}/items/{productId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResult<BasketDTO>>();
        result!.Data!.Items.First().Quantity.Should().Be(7);
        result.Data.TotalPrice.Should().Be(140_000); // 7 * 20,000
    }

    [Fact]
    public async Task UpdateItemQuantity_ShouldFail_WhenQuantityExceeds10()
    {
        // Arrange
        var userId = 106L;
        var productId = 6001;

        await _client.PostAsJsonAsync($"/api/v1/basket/{userId}/items",
            new AddBasketItemDTO(ProductId: productId, Quantity: 5, UnitPrice: 15_000));

        var updateRequest = new UpdateBasketItemQuantityRequest(11);

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/basket/{userId}/items/{productId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Content.ReadFromJsonAsync<ApiResult<BasketDTO>>();
        result.Errors.Should().NotBeNull();
    }

    [Fact]
    public async Task RemoveItem_ShouldRemoveSuccessfully()
    {
        // Arrange
        var userId = 107L;
        var productId = 7001;

        await _client.PostAsJsonAsync($"/api/v1/basket/{userId}/items",
            new AddBasketItemDTO(ProductId: productId, Quantity: 4, UnitPrice: 25_000));

        // Act
        var response = await _client.DeleteAsync($"/api/v1/basket/{userId}/items/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResult<BasketDTO>>();
        result!.Data!.Items.Should().BeEmpty();
        result.Data.TotalPrice.Should().Be(0);
    }

    [Fact]
    public async Task ClearBasket_ShouldClearAllItems()
    {
        // Arrange
        var userId = 108L;

        await _client.PostAsJsonAsync($"/api/v1/basket/{userId}/items",
            new AddBasketItemDTO(ProductId: 8001, Quantity: 2, UnitPrice: 30_000));
        await _client.PostAsJsonAsync($"/api/v1/basket/{userId}/items",
            new AddBasketItemDTO(ProductId: 8002, Quantity: 3, UnitPrice: 40_000));

        // Act
        var response = await _client.DeleteAsync($"/api/v1/basket/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ApiResult<BasketDTO>>();
        result!.Data!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task BasketOperations_ShouldInvalidateCache()
    {
        // Arrange
        var userId = 109L;
        var redis = _factory.GetRedisConnection().GetDatabase();
        var cacheKey = $"basket:{userId}";

        // افزودن آیتم (کش را پر می‌کند)
        await _client.PostAsJsonAsync($"/api/v1/basket/{userId}/items",
            new AddBasketItemDTO(ProductId: 9001, Quantity: 1, UnitPrice: 10_000));

        // کش باید خالی باشد
        var cachedValue = await redis.StringGetAsync(cacheKey);
        cachedValue.HasValue.Should().BeFalse();

        // دریافت سبد (کش را پر می‌کند)
        await _client.GetAsync($"/api/v1/basket/{userId}");

        // حالا کش باید پر باشد
        cachedValue = await redis.StringGetAsync(cacheKey);
        cachedValue.HasValue.Should().BeTrue();

        // افزودن آیتم دیگر (باید کش را پاک کند)
        await _client.PostAsJsonAsync($"/api/v1/basket/{userId}/items",
            new AddBasketItemDTO(ProductId: 9002, Quantity: 2, UnitPrice: 15_000));

        // Act & Assert - کش دوباره خالی شده
        cachedValue = await redis.StringGetAsync(cacheKey);
        cachedValue.HasValue.Should().BeFalse();
    }

    [Fact]
    public async Task AddItemToExpiredBasket_ShouldFail()
    {
        // Arrange
        var userId = 110L;

        // ایجاد سبد و افزودن آیتم
        await _client.PostAsJsonAsync($"/api/v1/basket/{userId}/items",
            new AddBasketItemDTO(ProductId: 10001, Quantity: 2, UnitPrice: 50_000));

        // منقضی کردن سبد به صورت مستقیم از دیتابیس
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var basket = await dbContext.Baskets
            .FirstAsync(b => b.UserId == userId && b.Status == Mansari.Store.Basket.Domain.Enums.BasketStatus.Active);

        basket.Expire();
        await dbContext.SaveChangesAsync();

        // Act - تلاش برای افزودن آیتم به سبد منقضی شده
        var response = await _client.PostAsJsonAsync($"/api/v1/basket/{userId}/items",
            new AddBasketItemDTO(ProductId: 10002, Quantity: 1, UnitPrice: 20_000));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Content.ReadFromJsonAsync<ApiResult<BasketDTO>>();
        result.Errors.Should().NotBeNull();
    }
}
