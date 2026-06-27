using Mansari.Store.Basket.Application.Common.Abstractions;
using Mansari.Store.Basket.Application.DTOs;

public sealed record ClearBasketCommand(long UserId)
    : ICommand<BasketDTO>;