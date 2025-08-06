// © 2025 Behrouz Rad. All rights reserved.

using Microsoft.EntityFrameworkCore;
using OrderService.Application.Persistence;
using OrderService.Domain.Orders;
using OrderService.Domain.ValueObjects;

namespace OrderService.Infrastructure.Data;

public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureOrderEntity(modelBuilder);
    }

    private static void ConfigureOrderEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);

            entity.Property(o => o.OrderNumber)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.HasIndex(o => o.OrderNumber)
                  .IsUnique();

            entity.Property(o => o.CreatedAt)
                .IsRequired();

            entity.Property(o => o.InvoiceAddress)
                .HasConversion(
                    v => v.Value,
                    v => InvoiceAddress.Create(v).Value)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(o => o.InvoiceEmailAddress)
                .HasConversion(
                    v => v.Value,
                    v => InvoiceEmailAddress.Create(v).Value)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(o => o.InvoiceCreditCardNumber)
                .HasConversion(
                    v => v.Value,
                    v => InvoiceCreditCardNumber.Create(v).Value)
                .IsRequired()
                .HasMaxLength(50);

            entity.OwnsMany(o => o.OrderItems, orderItem =>
            {
                orderItem.WithOwner().HasForeignKey("OrderId");
                orderItem.Property<int>("Id").ValueGeneratedOnAdd();
                orderItem.HasKey("Id");

                orderItem.Property(oi => oi.ProductId)
                         .IsRequired()
                         .HasMaxLength(50);

                orderItem.Property(oi => oi.ProductName)
                         .IsRequired()
                         .HasMaxLength(255);

                orderItem.Property(oi => oi.ProductAmount)
                         .IsRequired();

                orderItem.Property(oi => oi.ProductPrice)
                         .IsRequired()
                         .HasColumnType("decimal(18,2)");
            });
        });
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
