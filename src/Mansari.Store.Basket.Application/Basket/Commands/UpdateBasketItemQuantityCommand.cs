using Mansari.Store.Basket.Application.Common.Abstractions;
using Mansari.Store.Basket.Application.DTOs;

public sealed record UpdateBasketItemQuantityCommand(long UserId, long ProductId, int NewQuantity)
    : ICommand<BasketDTO>;