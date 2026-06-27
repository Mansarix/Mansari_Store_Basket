namespace Mansari.Store.Basket.Domain;

public static class BasketErrors
{
    public const string InvalidUserId = "شناسه کاربر معتبر نیست.";
    public const string InvalidProductId = "شناسه کالا معتبر نیست.";
    public const string InvalidQuantity = "تعداد هر کالا باید بین 1 تا 10 باشد.";
    public const string InvalidUnitPrice = "قیمت باید بیشتر از صفر باشد.";
    public const string BasketExpired = "این سبد خرید منقضی شده و قابل تغییر نیست.";
    public const string ItemNotFound = "آیتم موردنظر در سبد خرید پیدا نشد.";
    public const string TotalPriceExceeded = "مجموع قیمت سبد از سقف مجاز بیشتر شده است.";
}
