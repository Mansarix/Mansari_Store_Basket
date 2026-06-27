using FluentAssertions;
using Mansari.Store.Basket.API.Common.Results;
using Mansari.Store.Basket.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Json;
using Xunit;

namespace Mansari.Store.Basket.Tests.Integration;

public class BasketControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BasketControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetBasket_ShouldReturnSuccessAndValidDto()
    {
        // Arrange
        var userId = 1;

        // Act
        var response = await _client.GetAsync($"/api/v1/basket/{userId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<ApiResult<BasketDTO>>();

        content.Should().NotBeNull();
        content.Success.Should().BeTrue();
        content.Data.UserId.Should().Be(userId);
    }
}
