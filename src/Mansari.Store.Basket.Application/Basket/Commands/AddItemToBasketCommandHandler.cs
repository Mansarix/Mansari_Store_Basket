using Mansari.Store.Basket.Application.Basket.Common.Abstractions;
using Mansari.Store.Basket.Application.Basket.Mapping;
using Mansari.Store.Basket.Application.Common.Results;
using Mansari.Store.Basket.Application.DTOs;

namespace Mansari.Store.Basket.Application.Basket.Commands;

public class AddItemToBasketCommandHandler : ICommandHandler<AddItemToBasketCommand, BasketDTO>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IBasketCacheService _cacheService;
    private readonly IBasketEventPublisher _eventPublisher;

    public AddItemToBasketCommandHandler(
        IBasketRepository basketRepository,
        IBasketCacheService cacheService,
        IBasketEventPublisher eventPublisher)
    {
        _basketRepository = basketRepository;
        _cacheService = cacheService;
        _eventPublisher = eventPublisher;
    }

    public async Task<ServiceResult<BasketDTO>> Handle(
        AddItemToBasketCommand request,
        CancellationToken cancellationToken)
    {
        var basket = await _basketRepository.GetActiveBasketByUserIdAsync(request.UserId, cancellationToken)
                 ?? new Domain.Aggregates.Basket(request.UserId);


        if (basket.Id == 0)
        {
            await _basketRepository.AddAsync(basket, cancellationToken);
        }

        basket.AddItem(request.Item.ProductId, request.Item.Quantity, request.Item.UnitPrice);

        await _cacheService.RemoveAsync($"basket:{request.UserId}", cancellationToken);

        // Publishing event hndle in domain

        return ServiceResult<BasketDTO>.Success(
            basket.ToDto(),
            "Basket updated successfully");
    }
}
