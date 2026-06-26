namespace Mansari.Store.Basket.API.Contracts;

/// <summary>
/// درخواست تغییر تعداد کالا در سبد خرید
/// </summary>
public sealed record UpdateBasketItemQuantityRequest(
    int Quantity);