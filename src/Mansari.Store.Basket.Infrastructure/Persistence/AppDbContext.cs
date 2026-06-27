using Mansari.Store.Basket.Domain.Common;
using Mansari.Store.Basket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mansari.Store.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Mansari.Store.Basket.Domain.Aggregates.Basket> Baskets => Set<Mansari.Store.Basket.Domain.Aggregates.Basket>();
    public DbSet<BasketItem> BasketItems => Set<BasketItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public IEnumerable<IDomainEvent> GetDomainEvents()
    {
        return ChangeTracker
            .Entries<AggregateRoot<long>>()
            .Select(entry => entry.Entity)
            .SelectMany(entity => entity.DomainEvents)
            .ToList();
    }

    public void ClearDomainEvents()
    {
        foreach (var aggregate in ChangeTracker.Entries<AggregateRoot<long>>())
        {
            aggregate.Entity.ClearDomainEvents();
        }
    }
}