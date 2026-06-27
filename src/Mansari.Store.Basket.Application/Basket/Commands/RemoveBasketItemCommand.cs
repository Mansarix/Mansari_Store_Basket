using Mansari.Store.Basket.Application.Common.Abstractions;
using Mansari.Store.Basket.Application.DTOs;

public sealed record RemoveBasketItemCommand(long UserId, long ProductId)
    : ICommand<BasketDTO>;