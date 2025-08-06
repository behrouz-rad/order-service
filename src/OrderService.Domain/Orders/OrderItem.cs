// © 2025 Behrouz Rad. All rights reserved.

using FluentResults;

namespace OrderService.Domain.Orders;

public class OrderItem
{
    public int Id { get; init; }
    public string ProductId { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public int ProductAmount { get; init; }
    public decimal ProductPrice { get; init; }

    // Private constructor for EF Core
    private OrderItem() { }

    private OrderItem(string productId, string productName, int productAmount, decimal productPrice)
    {
        ProductId = productId;
        ProductName = productName;
        ProductAmount = productAmount;
        ProductPrice = productPrice;
    }

    public static Result<OrderItem> Create(string productId, string productName, int productAmount, decimal productPrice)
    {
        var errors = new List<IError>();

        if (string.IsNullOrWhiteSpace(productId))
        {
            errors.Add(new Error("Product ID cannot be null or empty"));
        }

        if (string.IsNullOrWhiteSpace(productName))
        {
            errors.Add(new Error("Product name cannot be null or empty"));
        }

        if (productAmount <= 0)
        {
            errors.Add(new Error("Product amount must be greater than zero"));
        }

        if (productPrice <= 0)
        {
            errors.Add(new Error("Product price must be greater than zero"));
        }

        if (errors.Count != 0)
        {
            return Result.Fail(errors);
        }

        return Result.Ok(new OrderItem(productId, productName, productAmount, productPrice));
    }
}
