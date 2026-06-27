using Mansari.Store.Basket.Application.Common.Abstractions;
using Mansari.Store.Basket.Application.DTOs;

namespace Mansari.Store.Basket.Application.Basket.Queries;

public sealed record GetOrCreateBasketQuery(long UserId)
    : IQuery<BasketDTO>;