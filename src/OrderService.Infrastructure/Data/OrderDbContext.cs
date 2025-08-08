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
