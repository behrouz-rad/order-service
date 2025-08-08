// © 2025 Behrouz Rad. All rights reserved.

using Microsoft.EntityFrameworkCore;
using OrderService.Application.Persistence;
using OrderService.Domain.Orders;

namespace OrderService.Infrastructure.Data;

public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
    }

    public DbSet<TEntity> DbSet<TEntity>() where TEntity : class => Set<TEntity>();

    public IQueryable<TEntity> DbSetAsNoTracking<TEntity>() where TEntity : class
        => Set<TEntity>().AsNoTracking();

    public new void Add<TEntity>(TEntity entity) where TEntity : class
        => Set<TEntity>().Add(entity);

    public new void Remove<TEntity>(TEntity entity) where TEntity : class
        => Set<TEntity>().Remove(entity);

    public void MarkAsChanged<TEntity>(TEntity entity) where TEntity : class
        => Update(entity);

    public new async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await base.SaveChangesAsync(cancellationToken);

    public void Migrate(TimeSpan timeout)
    {
        var pendingMigrations = Database.GetPendingMigrations();
        if (pendingMigrations.Any())
        {
            Database.SetCommandTimeout(timeout);
            Database.Migrate();
        }
    }
}
