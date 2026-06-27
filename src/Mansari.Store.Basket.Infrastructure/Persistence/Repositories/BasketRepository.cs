using Mansari.Store.Basket.Application.Basket.Common.Abstractions;
using Mansari.Store.Basket.Domain.Aggregates;
using Mansari.Store.Basket.Domain.Enums;
using Mansari.Store.Infrastructure.Persistence;
using Mansari.Store.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

public sealed class BasketRepository
    : Repository<Basket>,
      IBasketRepository
{
    public BasketRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<Basket?> GetActiveBasketByUserIdAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(x => x.Items)
            .Where(x => x.Status == BasketStatus.Active)
            .FirstOrDefaultAsync(
                x => x.UserId == userId,
                cancellationToken);
    }


    public async Task<List<Basket>>
        GetExpiredActiveBasketsAsync(
            DateTime olderThanUtc,
            CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(x => x.Items)
            .Where(x =>
                x.Status == BasketStatus.Active &&
                x.LastUpdatedAt < olderThanUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Basket>> GetExpiredCandidatesAsync(
        DateTime cutoff,
        CancellationToken cancellationToken)
    {
        return await DbSet
            .Include(x => x.Items)
            .Where(x =>
                x.Status == BasketStatus.Active &&
                x.LastUpdatedAt < cutoff)
            .ToListAsync(cancellationToken);
    }
}
