using Mansari.Store.Basket.Application.Common.Abstractions;
using Mansari.Store.Basket.Application.Common.Results;
using Mansari.Store.Basket.Application.DTOs;

public sealed record AddItemToBasketCommand(
    long UserId,
    AddBasketItemDTO Item)
    : ICommand<BasketDTO>;