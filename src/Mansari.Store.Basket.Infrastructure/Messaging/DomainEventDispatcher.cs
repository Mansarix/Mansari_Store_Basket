using Mansari.Store.Basket.Application.Basket.Common.Dispatchers;
using Mansari.Store.Infrastructure.Persistence;
using MediatR;

namespace Mansari.Store.Basket.Infrastructure.Messaging;

public sealed class DomainEventDispatcher(
    AppDbContext context,
    IMediator mediator)
    : IDomainEventDispatcher
{
    public async Task DispatchAsync(
        CancellationToken cancellationToken = default)
    {
        var domainEvents = context
            .GetDomainEvents()
            .ToList();

        context.ClearDomainEvents();

        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(
                domainEvent,
                cancellationToken);
        }
    }
}