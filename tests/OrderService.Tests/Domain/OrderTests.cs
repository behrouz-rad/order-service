// © 2025 Behrouz Rad. All rights reserved.

using FluentAssertions;
using OrderService.Domain.Entities;
using OrderService.Domain.ValueObjects;

namespace OrderService.Tests.Domain;

public record ValidOrderTestCase(
    string TestName,
    string OrderNumber,
    string AddressValue,
    string EmailValue,
    string CreditCardValue,
    ICollection<OrderItem> OrderItems)
{
    public override string ToString() => TestName;
}

public record InvalidOrderTestCase(
    string TestName,
    string? OrderNumber,
    string AddressValue,
    string EmailValue,
    string CreditCardValue,
    ICollection<OrderItem>? OrderItems,
    string ExpectedErrorMessage)
{
    public override string ToString() => TestName;
}

public class OrderTests
{
    public static TheoryData<ValidOrderTestCase> ValidOrderData =>
        [
            new ValidOrderTestCase(
                "Single item order with standard data",
                "20250801-ABCD1234",
                "123 Sample Street, 90402 Berlin",
                "customer@example.com",
                "1234-5678-9101-1121",
                new List<OrderItem> { new("12345", "Gaming Laptop", 2, 1499.99m) }
            ),
            new ValidOrderTestCase(
                "Multiple items order with different address",
                "20250801-WXYZ9876",
                "456 Main Avenue, 10001 New York",
                "john.doe@test.com",
                "9876-5432-1098-7654",
                new List<OrderItem>
                {
                    new("67890", "Wireless Mouse", 1, 29.99m),
                    new("54321", "Mechanical Keyboard", 1, 129.99m)
                }
            )
        ];

    [Theory]
    [MemberData(nameof(ValidOrderData))]
    public void Order_ShouldCreateValidOrder_WhenValidDataProvided(ValidOrderTestCase testCase)
    {
        // Arrange
        var invoiceAddress = new InvoiceAddress(testCase.AddressValue);
        var invoiceEmailAddress = new InvoiceEmailAddress(testCase.EmailValue);
        var invoiceCreditCardNumber = new InvoiceCreditCardNumber(testCase.CreditCardValue);

        // Act
        var order = new Order(testCase.OrderNumber, invoiceAddress, invoiceEmailAddress, invoiceCreditCardNumber, testCase.OrderItems);

        // Assert
        order.Should().NotBeNull();
        order.OrderNumber.Should().Be(testCase.OrderNumber);
        order.InvoiceAddress.Value.Should().Be(testCase.AddressValue);
        order.InvoiceEmailAddress.Value.Should().Be(testCase.EmailValue);
        order.InvoiceCreditCardNumber.Value.Should().Be(testCase.CreditCardValue);
        order.OrderItems.Should().HaveCount(testCase.OrderItems.Count);
        order.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    public static TheoryData<InvalidOrderTestCase> InvalidOrderData =>
        [
            new InvalidOrderTestCase(
                "Empty order number should throw exception",
                "",
                "123 Sample Street, 90402 Berlin",
                "customer@example.com",
                "1234-5678-9101-1121",
                new List<OrderItem> { new("12345", "Gaming Laptop", 1, 1499.99m) },
                "Order number cannot be null or empty*"
            ),
            new InvalidOrderTestCase(
                "Null order number should throw exception",
                null,
                "123 Sample Street, 90402 Berlin",
                "customer@example.com",
                "1234-5678-9101-1121",
                new List<OrderItem> { new("12345", "Gaming Laptop", 1, 1499.99m) },
                "Order number cannot be null or empty*"
            ),
            new InvalidOrderTestCase(
                "Empty order items list should throw exception",
                "20250801-ABCD1234",
                "123 Sample Street, 90402 Berlin",
                "customer@example.com",
                "1234-5678-9101-1121",
                new List<OrderItem>(),
                "Order must contain at least one item*"
            ),
            new InvalidOrderTestCase(
                "Null order items should throw exception",
                "20250801-ABCD1234",
                "123 Sample Street, 90402 Berlin",
                "customer@example.com",
                "1234-5678-9101-1121",
                null,
                "Order must contain at least one item*"
            )
        ];

    [Theory]
    [MemberData(nameof(InvalidOrderData))]
    public void Order_ShouldThrowArgumentException_WhenInvalidDataProvided(InvalidOrderTestCase testCase)
    {
        // Arrange
        var invoiceAddress = new InvoiceAddress(testCase.AddressValue);
        var invoiceEmailAddress = new InvoiceEmailAddress(testCase.EmailValue);
        var invoiceCreditCardNumber = new InvoiceCreditCardNumber(testCase.CreditCardValue);

        // Act & Assert
        var act = () => new Order(testCase.OrderNumber!, invoiceAddress, invoiceEmailAddress, invoiceCreditCardNumber, testCase.OrderItems!);
        act.Should().Throw<ArgumentException>()
            .WithMessage(testCase.ExpectedErrorMessage);
    }
}
