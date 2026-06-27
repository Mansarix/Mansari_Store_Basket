using Mansari.Store.Basket.Application.Basket.Common.Abstractions;
using Mansari.Store.Basket.Application.Basket.Mapping;
using Mansari.Store.Basket.Application.Common.Abstractions;
using Mansari.Store.Basket.Application.Common.Results;
using Mansari.Store.Basket.Application.DTOs;

namespace Mansari.Store.Basket.Application.Basket.Queries;

public sealed class GetOrCreateBasketQueryHandler(
    IBasketRepository basketRepository,
    IBasketCacheService cacheService,
    IUnitOfWork unitOfWork)
    : IQueryHandler<GetOrCreateBasketQuery, BasketDTO>
{
    private static string CacheKey(long userId) => $"basket:{userId}";

    public async Task<ServiceResult<BasketDTO>> Handle(
        GetOrCreateBasketQuery request,
        CancellationToken cancellationToken)
    {
        var cached = await cacheService.GetAsync<BasketDTO>(
            CacheKey(request.UserId),
            cancellationToken);

        if (cached is not null)
        {
            return ServiceResult<BasketDTO>.Success(cached);
        }

        var basket = await basketRepository.GetActiveBasketByUserIdAsync(request.UserId, cancellationToken);

        if (basket is null)
        {
            basket = new Mansari.Store.Basket.Domain.Aggregates.Basket(request.UserId);
            await basketRepository.AddAsync(basket, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var dto = basket.ToDto();
        await cacheService.SetAsync(CacheKey(request.UserId), dto, TimeSpan.FromMinutes(5), cancellationToken);

        return ServiceResult<BasketDTO>.Success(dto);
    }
}
