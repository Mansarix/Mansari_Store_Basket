using Mansari.Store.Basket.Domain.Common;
using Mansari.Store.Basket.Domain.Rules;
using Mansari.Store.Basket.Domain.ValueObjects;

namespace Mansari.Store.Basket.Domain.Entities;

public sealed class BasketItem : Entity<long>
{
    public long BasketId { get; private set; }

    public long ProductId { get; private set; }

    public Quantity Quantity { get; private set; } = null!;

    public decimal UnitPrice { get; private set; }

    public decimal TotalPrice => Quantity.Value * UnitPrice;

    private BasketItem() { }

    internal BasketItem(long productId, int quantity, decimal unitPrice)
    {
        ProductId = productId;
        SetQuantity(quantity);
        SetUnitPrice(unitPrice);
    }

    internal void SetQuantity(int quantity)
    {
        if (quantity < 1 || quantity > BasketRules.MaxItemQuantity)
        {
            throw new BasketDomainException(BasketErrors.InvalidQuantity);
        }

        Quantity = Quantity.Create(quantity);
    }

    internal void SetUnitPrice(decimal unitPrice)
    {
        if (unitPrice <= 0)
        {
            throw new BasketDomainException(BasketErrors.InvalidUnitPrice);
        }

        UnitPrice = unitPrice;
    }
}
