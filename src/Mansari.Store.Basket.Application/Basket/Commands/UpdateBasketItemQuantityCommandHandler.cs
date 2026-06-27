using Mansari.Store.Basket.Application.Basket.Common.Abstractions;
using Mansari.Store.Basket.Application.Basket.Mapping;
using Mansari.Store.Basket.Application.Common.Results;
using Mansari.Store.Basket.Application.DTOs;
using Mansari.Store.Basket.Domain.Events;

namespace Mansari.Store.Basket.Application.Basket.Commands;

public class UpdateBasketItemQuantityCommandHandler
    : ICommandHandler<UpdateBasketItemQuantityCommand, BasketDTO>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IBasketCacheService _cacheService;
    private readonly IBasketEventPublisher _eventPublisher;

    public UpdateBasketItemQuantityCommandHandler(
        IBasketRepository basketRepository,
        IBasketCacheService cacheService,
        IBasketEventPublisher eventPublisher)
    {
        _basketRepository = basketRepository;
        _cacheService = cacheService;
        _eventPublisher = eventPublisher;
    }

    public async Task<ServiceResult<BasketDTO>> Handle(
        UpdateBasketItemQuantityCommand request,
        CancellationToken cancellationToken)
    {
        var basket = await _basketRepository.GetActiveBasketByUserIdAsync(request.UserId, cancellationToken);

        if (basket is null)
        {
            return ServiceResult<BasketDTO>.Failure("Basket not found.", 404);
        }

        basket.UpdateQuantity( request.ProductId, request.NewQuantity);

        await _cacheService.RemoveAsync($"basket:{request.UserId}", cancellationToken);

        // Publish event
        await _eventPublisher.PublishAsync(new BasketItemQuantityUpdatedEvent
        {
            BasketId = basket.Id,
            UserId = basket.UserId,
            ProductId = request.ProductId,
            NewQuantity = request.NewQuantity,
            OccurredOnUtc = DateTime.UtcNow
        }, cancellationToken);

        return ServiceResult<BasketDTO>.Success(basket.ToDto());
    }
}
