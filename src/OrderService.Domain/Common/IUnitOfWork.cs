// © 2025 Behrouz Rad. All rights reserved.

using Microsoft.EntityFrameworkCore;

namespace OrderService.Domain.Common;

public interface IUnitOfWork : IDisposable
{
    public DbSet<TEntity> DbSet<TEntity>() where TEntity : class;
    public IQueryable<TEntity> DbSetAsNoTracking<TEntity>() where TEntity : class;
    public void Add<TEntity>(TEntity entity) where TEntity : class;
    public void Remove<TEntity>(TEntity entity) where TEntity : class;
    public void MarkAsChanged<TEntity>(TEntity entity) where TEntity : class;
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    public void Migrate(TimeSpan timeout);
}
