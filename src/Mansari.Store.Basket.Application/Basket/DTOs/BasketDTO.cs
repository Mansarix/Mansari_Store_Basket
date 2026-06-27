using Mansari.Store.Basket.Application.Basket.DTOs;

namespace Mansari.Store.Basket.Application.DTOs;

public sealed record BasketDTO(
    long Id,
    long UserId,
    string Status,
    IReadOnlyCollection<BasketItemDTO> Items,
    decimal TotalPrice,
    DateTime CreatedAt,
    DateTime LastUpdatedAt);