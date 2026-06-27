using Mansari.Store.Basket.Domain.Common;

namespace Mansari.Store.Basket.Domain.Events;

public class BasketClearedEvent : IDomainEvent
{
    public BasketClearedEvent( long userId, int itemCount, decimal totalPrice)
    {
        UserId = userId;
        ItemCount = itemCount;
        TotalPrice = totalPrice;
    }
    public BasketClearedEvent()
    {
    }

    public BasketClearedEvent(long id, long userId, int itemCount, decimal totalPrice)
    {
        Id = id;
        UserId = userId;
        ItemCount = itemCount;
        TotalPrice = totalPrice;
    }

    public long BasketId { get; set; }
    public long UserId { get; set; }
    public DateTime OccurredOnUtc { get; set; }
    public int ItemCount { get; }
    public decimal TotalPrice { get; }
    public long Id { get; }
}
