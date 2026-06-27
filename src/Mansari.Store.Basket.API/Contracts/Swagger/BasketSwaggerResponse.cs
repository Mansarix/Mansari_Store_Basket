namespace Mansari.Store.Basket.API.Contracts.Swagger;

/// <summary>
/// مدل نمایشی سبد خرید در مستندات Swagger.
/// </summary>
public sealed class BasketSwaggerResponse
{
    /// <summary>
    /// شناسه سبد خرید.
    /// </summary>
    /// <example>1001</example>
    public long Id { get; init; }

    /// <summary>
    /// شناسه کاربر مالک سبد خرید.
    /// </summary>
    /// <example>25</example>
    public long UserId { get; init; }

    /// <summary>
    /// وضعیت فعلی سبد خرید.
    /// </summary>
    /// <example>Active</example>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// لیست کالاهای موجود در سبد خرید.
    /// </summary>
    public IReadOnlyCollection<BasketItemSwaggerResponse> Items { get; init; } = [];

    /// <summary>
    /// مبلغ کل سبد خرید.
    /// </summary>
    /// <example>450000</example>
    public decimal TotalPrice { get; init; }

    /// <summary>
    /// تاریخ ایجاد سبد خرید.
    /// </summary>
    /// <example>2026-06-27T10:30:00Z</example>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// تاریخ آخرین به‌روزرسانی سبد خرید.
    /// </summary>
    /// <example>2026-06-27T11:15:00Z</example>
    public DateTime LastUpdatedAt { get; init; }
}
