namespace Mansari.Store.Basket.Application.Basket.Common.Abstractions;

public interface IBasketRepository : IRepository<Domain.Aggregates.Basket>
{
    Task<Domain.Aggregates.Basket?> GetActiveBasketByUserIdAsync(long userId, CancellationToken cancellationToken = default);
    // فعلا نیازش نداریم ، برای برگرداندن سبد های غیرفعال
    // Task<Domain.Aggregates.Basket?> GetAllBasketsByUserIdAsync(long userId, CancellationToken cancellationToken);
    Task<List<Domain.Aggregates.Basket>> GetExpiredActiveBasketsAsync(DateTime olderThanUtc, CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Aggregates.Basket>> GetExpiredCandidatesAsync(DateTime cutoff, CancellationToken cancellationToken);
}
