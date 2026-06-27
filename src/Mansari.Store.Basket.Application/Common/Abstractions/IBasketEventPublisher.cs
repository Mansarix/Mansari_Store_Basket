namespace Mansari.Store.Basket.Application.Basket.Common.Abstractions;

public interface IBasketEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class;
}
