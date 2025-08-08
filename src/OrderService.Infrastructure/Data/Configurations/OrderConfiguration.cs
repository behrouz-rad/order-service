// © 2025 Behrouz Rad. All rights reserved.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Orders;
using OrderService.Domain.ValueObjects;

namespace OrderService.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderNumber)
               .IsRequired()
               .HasMaxLength(50);

        builder.HasIndex(o => o.OrderNumber)
               .IsUnique();

        builder.Property(o => o.CreatedAt)
               .IsRequired();

        // Configure value objects with conversions
        builder.Property(o => o.InvoiceAddress)
               .HasConversion(
                   v => v.Value,
                   v => InvoiceAddress.Create(v).Value)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(o => o.InvoiceEmailAddress)
               .HasConversion(
                   v => v.Value,
                   v => InvoiceEmailAddress.Create(v).Value)
               .IsRequired()
               .HasMaxLength(256);

        builder.Property(o => o.InvoiceCreditCardNumber)
               .HasConversion(
                   v => v.Value,
                   v => InvoiceCreditCardNumber.Create(v).Value)
               .IsRequired()
               .HasMaxLength(50);

        // Configure owned entity for OrderItems
        builder.OwnsMany(o => o.OrderItems, orderItem =>
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

        // Table mapping
        builder.ToTable("Orders");
    }
}
