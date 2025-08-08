// © 2025 Behrouz Rad. All rights reserved.

using FluentAssertions;
using OrderService.Domain.Orders;

namespace OrderService.UnitTests.Domain.Factories;

public class OrderFactoryTests
{
    [Fact]
    public void Create_WithValidData_ShouldReturnSuccessResult()
    {
        // Arrange
        const string orderNumber = "20250801-ABCD1234";
        const string invoiceAddress = "123 Sample Street, 90402 Berlin";
        const string invoiceEmailAddress = "customer@example.com";
        const string invoiceCreditCardNumber = "1234-5678-9101-1121";
        (string, string, int, decimal)[] orderItems =
        [
            ("12345", "Gaming Laptop", 2, 1499.99m),
            ("67890", "Wireless Mouse", 1, 29.99m)
        ];

        // Act
        FluentResults.Result<Order> result = OrderFactory.Create(
            orderNumber,
            invoiceAddress,
            invoiceEmailAddress,
            invoiceCreditCardNumber,
            orderItems);

        // Assert
        result.IsSuccess.Should().BeTrue();
        Order order = result.Value;
        order.Should().NotBeNull();
        order.OrderNumber.Should().Be(orderNumber);
        order.InvoiceAddress.Value.Should().Be(invoiceAddress);
        order.InvoiceEmailAddress.Value.Should().Be(invoiceEmailAddress.ToLowerInvariant());
        order.InvoiceCreditCardNumber.Value.Should().Be(invoiceCreditCardNumber);
        order.OrderItems.Should().HaveCount(2);
        order.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_WithInvalidEmailAddress_ShouldReturnFailureResult()
    {
        // Arrange
        const string orderNumber = "20250801-ABCD1234";
        const string invoiceAddress = "123 Sample Street, 90402 Berlin";
        const string invoiceEmailAddress = "invalid-email";
        const string invoiceCreditCardNumber = "1234-5678-9101-1121";
        (string, string, int, decimal)[] orderItems = [("12345", "Gaming Laptop", 2, 1499.99m)];

        // Act
        FluentResults.Result<Order> result = OrderFactory.Create(
            orderNumber,
            invoiceAddress,
            invoiceEmailAddress,
            invoiceCreditCardNumber,
            orderItems);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("Invalid email format"));
    }

    [Fact]
    public void Create_WithInvalidAddress_ShouldReturnFailureResult()
    {
        // Arrange
        const string orderNumber = "20250801-ABCD1234";
        const string invoiceAddress = "Short";
        const string invoiceEmailAddress = "customer@example.com";
        const string invoiceCreditCardNumber = "1234-5678-9101-1121";
        (string, string, int, decimal)[] orderItems = [("12345", "Gaming Laptop", 2, 1499.99m)];

        // Act
        FluentResults.Result<Order> result = OrderFactory.Create(
            orderNumber,
            invoiceAddress,
            invoiceEmailAddress,
            invoiceCreditCardNumber,
            orderItems);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("must be at least 10 characters long"));
    }

    [Fact]
    public void Create_WithInvalidOrderItem_ShouldReturnFailureResult()
    {
        // Arrange
        const string orderNumber = "20250801-ABCD1234";
        const string invoiceAddress = "123 Sample Street, 90402 Berlin";
        const string invoiceEmailAddress = "customer@example.com";
        const string invoiceCreditCardNumber = "1234-5678-9101-1121";
        (string, string, int, decimal)[] orderItems = [("", "Gaming Laptop", 2, 1499.99m)];

        // Act
        FluentResults.Result<Order> result = OrderFactory.Create(
            orderNumber,
            invoiceAddress,
            invoiceEmailAddress,
            invoiceCreditCardNumber,
            orderItems);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Message.Contains("Product ID cannot be null or empty"));
    }

    [Fact]
    public void Create_WithMultipleValidationErrors_ShouldReturnIndividualErrors()
    {
        // Arrange
        const string orderNumber = "20250801-ABCD1234";
        const string invoiceAddress = "Short";
        const string invoiceEmailAddress = "invalid-email";
        const string invoiceCreditCardNumber = "123";
        (string, string, int, decimal)[] orderItems = [("", "", -1, -10.0m)];

        // Act
        FluentResults.Result<Order> result = OrderFactory.Create(
            orderNumber,
            invoiceAddress,
            invoiceEmailAddress,
            invoiceCreditCardNumber,
            orderItems);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().HaveCountGreaterThan(3);
        result.Errors.Should().Contain(e => e.Message.Contains("must be at least 10 characters long"));
        result.Errors.Should().Contain(e => e.Message.Contains("Invalid email format"));
        result.Errors.Should().Contain(e => e.Message.Contains("Product ID cannot be null or empty"));
    }
}
