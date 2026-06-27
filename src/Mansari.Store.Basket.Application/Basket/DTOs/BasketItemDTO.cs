namespace Mansari.Store.Basket.Application.DTOs;

public sealed record BasketItemDTO(
    long ProductId,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice);