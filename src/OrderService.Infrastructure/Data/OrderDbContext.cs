// © 2025 Behrouz Rad. All rights reserved.

using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Common;
using OrderService.Domain.Entities;
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
                    v => new InvoiceAddress(v))
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(o => o.InvoiceEmailAddress)
                .HasConversion(
                    v => v.Value,
                    v => new InvoiceEmailAddress(v))
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(o => o.InvoiceCreditCardNumber)
                .HasConversion(
                    v => v.Value,
                    v => new InvoiceCreditCardNumber(v))
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

                orderItem.Ignore(oi => oi.TotalPrice);
            });
        });
    }

    public DbSet<TEntity> DbSet<TEntity>() where TEntity : class => Set<TEntity>();

    public IQueryable<TEntity> DbSetAsNoTracking<TEntity>() where TEntity : class
        => Set<TEntity>().AsNoTracking();

    public new void Add<TEntity>(TEntity entity) where TEntity : class
        => Entry(entity).State = EntityState.Added;

    public new void Remove<TEntity>(TEntity entity) where TEntity : class
        => Entry(entity).State = EntityState.Deleted;

    public void MarkAsChanged<TEntity>(TEntity entity) where TEntity : class
        => Entry(entity).State = EntityState.Modified;

    public new async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await base.SaveChangesAsync(cancellationToken);

    public void Migrate(TimeSpan timeout)
    {
        Database.SetCommandTimeout(timeout);
        Database.Migrate();
    }
}
