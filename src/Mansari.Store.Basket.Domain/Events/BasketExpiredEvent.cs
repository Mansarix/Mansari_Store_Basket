using Mansari.Store.Basket.Domain.Common;

public sealed class BasketExpiredEvent : IDomainEvent
{
    public BasketExpiredEvent(long basketId, long userId)
    {
        BasketId = basketId;
        UserId = userId;
        OccurredOnUtc = DateTime.UtcNow;
    }

    public long BasketId { get; }
    public long UserId { get; }
    public DateTime OccurredOnUtc { get; }
}
