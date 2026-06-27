namespace Mansari.Store.Basket.Domain.Rules;

public static class BasketRules
{
    public const int MaxItemQuantity = 10;

    public const decimal MaxTotalPrice = 50_000_000m;

    public const int BasketExpirationMinutes = 30;
}
