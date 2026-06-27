using Mansari.Store.Basket.Application.Basket.Common.Abstractions;
using Mansari.Store.Basket.Application.Common.Abstractions;
using Mansari.Store.Basket.Application.Common.Results;
using Mansari.Store.Basket.Domain.Rules;

namespace Mansari.Store.Basket.Application.Basket.Commands;

public class ExpireBasketsCommandHandler : ICommandHandler<ExpireBasketsCommand, int>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IBasketCacheService _cacheService;
    private readonly IBasketEventPublisher _eventPublisher;

    public ExpireBasketsCommandHandler(
        IBasketRepository basketRepository,
        IBasketCacheService cacheService,
        IBasketEventPublisher eventPublisher)
    {
        _basketRepository = basketRepository;
        _cacheService = cacheService;
        _eventPublisher = eventPublisher;
    }

    public async Task<ServiceResult<int>> Handle(
        ExpireBasketsCommand request,
        CancellationToken cancellationToken)
    {
        var cutoff = DateTime.UtcNow.AddMinutes(-BasketRules.BasketExpirationMinutes); // 30 minutes cutoff
        var baskets = await _basketRepository.GetExpiredCandidatesAsync(cutoff, cancellationToken);

        foreach (var basket in baskets)
        {
            basket.Expire();
            await _cacheService.RemoveAsync($"basket:{basket.UserId}", cancellationToken);
        }

        // Publish domain events that were raised by Expire()
        foreach (var basket in baskets)
        {
            var domainEvents = basket.DomainEvents;
            foreach (var domainEvent in domainEvents)
            {
                await _eventPublisher.PublishAsync(domainEvent, cancellationToken);
            }
        }

        return ServiceResult<int>.Success(baskets.Count());
    }
}
