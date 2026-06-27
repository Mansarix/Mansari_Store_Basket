using Mansari.Store.Basket.Application.Basket.Common.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Mansari.Store.Infrastructure.Persistence.Repositories.Base;

public abstract class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> DbSet;

    protected Repository(AppDbContext context)
    {
        Context = context;
        DbSet = Context.Set<T>();
    }

    public virtual Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => DbSet.FindAsync([id], cancellationToken).AsTask();

    public virtual Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => DbSet.AddAsync(entity, cancellationToken).AsTask();

    public virtual void Update(T entity)
        => DbSet.Update(entity);

    public virtual void Remove(T entity)
        => DbSet.Remove(entity);

    public virtual IQueryable<T> Query()
        => DbSet.AsQueryable();
}