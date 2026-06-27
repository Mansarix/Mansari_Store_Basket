namespace Mansari.Store.Basket.Application.DTOs;

public sealed record AddBasketItemDTO(long ProductId, int Quantity, decimal UnitPrice);
