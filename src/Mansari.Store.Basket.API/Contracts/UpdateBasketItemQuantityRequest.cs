namespace Mansari.Store.Basket.API.Contracts;

/// <summary>
/// درخواست تغییر تعداد یک کالا در سبد خرید.
/// </summary>
public sealed class UpdateBasketItemQuantityRequest
{
    public UpdateBasketItemQuantityRequest(int newQuantityValue)
    {
        Quantity = newQuantityValue;
    }

    /// <summary>
    /// تعداد جدید کالا در سبد خرید.
    /// </summary>
    /// <example>3</example>
    public int Quantity { get; init; }
}