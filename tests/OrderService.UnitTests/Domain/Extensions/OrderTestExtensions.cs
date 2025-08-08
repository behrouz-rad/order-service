// © 2025 Behrouz Rad. All rights reserved.

using OrderService.Domain.Orders;

namespace OrderService.UnitTests.Domain.Extensions;

public static class OrderTestExtensions
{
    /// <summary>
    /// Creates a valid order for testing purposes using the OrderFactory
    /// </summary>
    public static Order CreateTestOrder(
        string orderNumber = "TEST-001",
        string address = "123 Test Street, 90210 Test City",
        string email = "test@example.com",
        string creditCard = "1234-5678-9101-1121",
        params (string ProductId, string ProductName, int Amount, decimal Price)[] items)
    {
        var orderItems = items.Length != 0 ? items : new[] { ("TEST-001", "Test Product", 1, 99.99m) };
        
        var result = OrderFactory.Create(orderNumber, address, email, creditCard, orderItems);
        
        if (result.IsFailed)
        {
            throw new InvalidOperationException($"Failed to create test order: {string.Join(", ", result.Errors.Select(e => e.Message))}");
        }
        
        return result.Value;
    }

    /// <summary>
    /// Creates an order with specific validation errors for testing error scenarios
    /// </summary>
    public static FluentResults.Result<Order> CreateInvalidOrder(
        string? orderNumber = null,
        string? address = null,
        string? email = null,
        string? creditCard = null,
        (string ProductId, string ProductName, int Amount, decimal Price)[]? items = null)
    {
        return OrderFactory.Create(
            orderNumber ?? "",
            address ?? "",
            email ?? "",
            creditCard ?? "",
            items ?? [("", "", 0, 0m)]);
    }
}
