using Mansari.Store.Basket.Domain.Common;
using Mansari.Store.Basket.Domain.Entities;
using Mansari.Store.Basket.Domain.Enums;
using Mansari.Store.Basket.Domain.Events;
using Mansari.Store.Basket.Domain.Rules;

namespace Mansari.Store.Basket.Domain.Aggregates;

public class Basket : AggregateRoot<long>
{
    public long UserId { get; private set; }
    public BasketStatus Status { get; private set; }
    private readonly List<BasketItem> _items = new();
    public IReadOnlyCollection<BasketItem> Items => _items.AsReadOnly();
    public DateTime CreatedAt { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }

    public decimal TotalPrice => _items.Sum(i => i.TotalPrice);
    public bool IsEmpty => !_items.Any();

    private Basket() { }

    public Basket(long userId)
    {
        if (userId <= 0)
            throw new DomainException(BasketErrors.InvalidUserId);

        UserId = userId;
        Status = BasketStatus.Active;
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new BasketCreatedEvent());
    }

    public void AddItem(long productId, int quantity, decimal unitPrice)
    {
        EnsureActive();
        ValidateProductId(productId);

        if (quantity < 1 || quantity > BasketRules.MaxItemQuantity)
            throw new DomainException(BasketErrors.InvalidQuantity);

        if (unitPrice <= 0)
            throw new DomainException(BasketErrors.InvalidUnitPrice);

        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);

        if (existingItem != null)
        {
            var newQuantity = existingItem.Quantity + quantity;
            if (newQuantity > BasketRules.MaxItemQuantity)
                throw new DomainException(BasketErrors.InvalidQuantity);

            var newTotal = TotalPrice - existingItem.TotalPrice + (newQuantity * unitPrice);
            EnsureTotalPriceLimit(newTotal);

            var oldQuantity = existingItem.Quantity;
            existingItem.SetQuantity(newQuantity);
            existingItem.SetUnitPrice(unitPrice);

            AddDomainEvent(new BasketItemQuantityUpdatedEvent(Id, UserId, productId, oldQuantity, newQuantity));
        }
        else
        {
            var newItem = new BasketItem(productId, quantity, unitPrice);
            var newTotal = TotalPrice + newItem.TotalPrice;
            EnsureTotalPriceLimit(newTotal);

            _items.Add(newItem);
            AddDomainEvent(new BasketItemAddedEvent(Id, UserId, productId, quantity, unitPrice));
        }

        Touch();
    }

    public void UpdateQuantity(long productId, int newQuantity)
    {
        EnsureActive();
        ValidateProductId(productId);

        if (newQuantity < 1 || newQuantity > BasketRules.MaxItemQuantity)
            throw new DomainException(BasketErrors.InvalidQuantity);
       
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        
        if (item == null)
            throw new DomainException(BasketErrors.ItemNotFound);

        if (item.UnitPrice <= 0)
            throw new DomainException(BasketErrors.InvalidUnitPrice);

        var newTotal =
            TotalPrice
            - item.TotalPrice
            + (newQuantity * item.UnitPrice);

        EnsureTotalPriceLimit(newTotal);

        var oldQuantity = item.Quantity;
        item.SetQuantity(newQuantity);

        AddDomainEvent(new BasketItemQuantityUpdatedEvent(Id, UserId, productId, oldQuantity, newQuantity));
        Touch();
    }

    public void RemoveItem(long productId)
    {
        EnsureActive();
        ValidateProductId(productId);

        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
            throw new DomainException(BasketErrors.ItemNotFound);

        _items.Remove(item);
        AddDomainEvent(new BasketItemRemovedEvent(Id, UserId, productId, item.Quantity, item.UnitPrice));
        Touch();
    }

    public void Clear()
    {
        EnsureActive();

        if (IsEmpty)
            return;

        var itemCount = _items.Count;
        var totalPrice = TotalPrice;
        _items.Clear();

        AddDomainEvent(new BasketClearedEvent(Id, UserId, itemCount, totalPrice));
        Touch();
    }

    public void Expire()
    {
        if (Status == BasketStatus.Expired)
            return;

        Status = BasketStatus.Expired;
        Touch();

        AddDomainEvent(new BasketExpiredEvent(Id, UserId));
    }

    private void Touch()
    {
        LastUpdatedAt = DateTime.UtcNow;
    }

    private void ValidateProductId(long productId)
    {
        if (productId <= 0)
            throw new DomainException(BasketErrors.InvalidProductId);
    }

    private void EnsureTotalPriceLimit(decimal newTotal)
    {
        if (newTotal > BasketRules.MaxTotalPrice)
            throw new DomainException(BasketErrors.TotalPriceExceeded);
    }

    private void EnsureActive()
    {
        if (Status == BasketStatus.Expired)
            throw new DomainException(BasketErrors.BasketExpired);
    }
}
