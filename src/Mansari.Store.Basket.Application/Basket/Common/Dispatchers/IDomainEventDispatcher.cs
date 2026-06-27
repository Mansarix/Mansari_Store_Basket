namespace Mansari.Store.Basket.Application.Basket.Common.Dispatchers;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(
        CancellationToken cancellationToken = default);
}