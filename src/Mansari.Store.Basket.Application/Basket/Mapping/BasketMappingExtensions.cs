using Mansari.Store.Basket.Application.Basket.DTOs;
using Mansari.Store.Basket.Application.DTOs;
using Mansari.Store.Basket.Domain.Entities;

namespace Mansari.Store.Basket.Application.Basket.Mapping;

public static class BasketMappingExtensions
{
    public static BasketDTO ToDto(this Mansari.Store.Basket.Domain.Aggregates.Basket basket)
    {
        var items = basket.Items
            .Select(x => new BasketItemDTO(
                x.ProductId,
                x.Quantity,
                x.UnitPrice,
                x.TotalPrice))
            .ToList();

        return new BasketDTO(
            basket.Id,
            basket.UserId,
            basket.Status.ToString(),
            items,
            basket.TotalPrice,
            basket.CreatedAt,
            basket.LastUpdatedAt);
    }
}
