using Mansari.Store.Basket.Domain.Common;
using Mansari.Store.Basket.Domain.ValueObjects;

namespace Mansari.Store.Basket.Domain.Events;

public class BasketItemQuantityUpdatedEvent : IDomainEvent
{
    public BasketItemQuantityUpdatedEvent()
    {
        
    }
    public BasketItemQuantityUpdatedEvent(long userId, long productId, Quantity oldQuantity, int newQuantity)
    {
        UserId = userId;
        ProductId = productId;
        OldQuantity = oldQuantity;
        NewQuantity = newQuantity;
    }

    public BasketItemQuantityUpdatedEvent(long id, long userId, long productId, Quantity oldQuantity, int newQuantity)
    {
        Id = id;
        UserId = userId;
        ProductId = productId;
        OldQuantity = oldQuantity;
        NewQuantity = newQuantity;
    }

    public long BasketId { get; set; }
    public long UserId { get; set; }
    public long ProductId { get; set; }
    public int NewQuantity { get; set; }
    public DateTime OccurredOnUtc { get; set; }
    public Quantity OldQuantity { get; }
    public long Id { get; }
}
