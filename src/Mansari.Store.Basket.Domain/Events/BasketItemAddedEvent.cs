using Mansari.Store.Basket.Domain.Common;

namespace Mansari.Store.Basket.Domain.Events
{
    internal class BasketItemAddedEvent : IDomainEvent
    {
        public BasketItemAddedEvent(long id, long userId, long productId, int quantity, decimal unitPrice)
        {
            Id = id;
            UserId = userId;
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        public long Id { get; }
        public long UserId { get; }
        public long ProductId { get; }
        public int Quantity { get; }
        public decimal UnitPrice { get; }
    }
}