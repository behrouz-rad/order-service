// © 2025 Behrouz Rad. All rights reserved.

namespace OrderService.Domain.ValueObjects;

public record OrderItem
{
    public int Id { get; init; }
    public string ProductId { get; init; }
    public string ProductName { get; init; }
    public int ProductAmount { get; init; }
    public decimal ProductPrice { get; init; }

    public OrderItem(string productId, string productName, int productAmount, decimal productPrice)
    {
        if (string.IsNullOrWhiteSpace(productId))
        {
            throw new ArgumentException("Product ID cannot be null or empty", nameof(productId));
        }

        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new ArgumentException("Product name cannot be null or empty", nameof(productName));
        }

        if (productAmount <= 0)
        {
            throw new ArgumentException("Product amount must be greater than zero", nameof(productAmount));
        }

        if (productPrice <= 0)
        {
            throw new ArgumentException("Product price must be greater than zero", nameof(productPrice));
        }

        ProductId = productId;
        ProductName = productName;
        ProductAmount = productAmount;
        ProductPrice = productPrice;
    }
}
