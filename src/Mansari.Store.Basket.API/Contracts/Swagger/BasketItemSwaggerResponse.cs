namespace Mansari.Store.Basket.API.Contracts.Swagger;

/// <summary>
/// مدل نمایشی کالای موجود در سبد خرید در مستندات Swagger.
/// </summary>
public sealed class BasketItemSwaggerResponse
{
    /// <summary>
    /// شناسه محصول.
    /// </summary>
    /// <example>501</example>
    public long ProductId { get; init; }

    /// <summary>
    /// تعداد کالا در سبد خرید.
    /// </summary>
    /// <example>2</example>
    public int Quantity { get; init; }

    /// <summary>
    /// قیمت واحد کالا.
    /// </summary>
    /// <example>225000</example>
    public decimal UnitPrice { get; init; }

    /// <summary>
    /// مبلغ کل این آیتم بر اساس تعداد و قیمت واحد.
    /// </summary>
    /// <example>450000</example>
    public decimal TotalPrice { get; init; }
}
