using Mansari.Store.Basket.Domain.Common;
using Mansari.Store.Basket.Domain.ValueObjects;

namespace Mansari.Store.Basket.Domain.Events;

public class BasketItemRemovedEvent : IDomainEvent
{
    public BasketItemRemovedEvent()
    {
        
    }
    public BasketItemRemovedEvent(long userId, long productId, Quantity quantity, decimal unitPrice)
    {
        UserId = userId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public BasketItemRemovedEvent(long id, long userId, long productId, Quantity quantity, decimal unitPrice)
    {
        Id = id;
        UserId = userId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public long BasketId { get; set; }
    public long UserId { get; set; }
    public long ProductId { get; set; }
    public DateTime OccurredOnUtc { get; set; }
    public Quantity Quantity { get; }
    public decimal UnitPrice { get; }
    public long Id { get; }
}
