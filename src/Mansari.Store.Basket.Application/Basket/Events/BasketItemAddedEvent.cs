// BasketItemAddedEvent.cs
namespace Mansari.Store.Basket.Application.Basket.Events;

public class BasketItemAddedEvent
{
    public long BasketId { get; set; }
    public long UserId { get; set; }
    public long ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime OccurredOnUtc { get; set; }
}
