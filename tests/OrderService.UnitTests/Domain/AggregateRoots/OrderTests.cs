// © 2025 Behrouz Rad. All rights reserved.

using FluentAssertions;
using OrderService.Domain.Orders;
using OrderService.Domain.ValueObjects;
using OrderService.UnitTests.Domain.Extensions;

namespace OrderService.UnitTests.Domain.AggregateRoots;

public sealed record ValidOrderTestCase(
    string TestName,
    string OrderNumber,
    string AddressValue,
    string EmailValue,
    string CreditCardValue,
    ICollection<OrderItem> OrderItems)
{
    public override string ToString() => TestName;
}

public sealed record InvalidOrderTestCase(
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

public sealed class OrderTests
{
    public static TheoryData<ValidOrderTestCase> ValidOrderData =>
        [
            new ValidOrderTestCase(
                "Single item order with standard data",
                "20250801-ABCD1234",
                "123 Sample Street, 90402 Berlin",
                "customer@example.com",
                "1234-5678-9101-1121",
                new List<OrderItem> { OrderItem.Create("12345", "Gaming Laptop", 2, 1499.99m).Value }
            ),
            new ValidOrderTestCase(
                "Multiple items order with different address",
                "20250801-WXYZ9876",
                "456 Main Avenue, 10001 New York",
                "john.doe@test.com",
                "9876-5432-1098-7654",
                new List<OrderItem>
                {
                    OrderItem.Create("67890", "Wireless Mouse", 1, 29.99m).Value,
                    OrderItem.Create("54321", "Mechanical Keyboard", 1, 129.99m).Value
                }
            )
        ];

    [Theory]
    [MemberData(nameof(ValidOrderData))]
    public void Order_ShouldCreateValidOrder_WhenValidDataProvided(ValidOrderTestCase testCase)
    {
        // Arrange
        var invoiceAddressResult = InvoiceAddress.Create(testCase.AddressValue);
        var invoiceEmailAddressResult = InvoiceEmailAddress.Create(testCase.EmailValue);
        var invoiceCreditCardNumberResult = InvoiceCreditCardNumber.Create(testCase.CreditCardValue);

        // Act
        var orderResult = Order.Create(testCase.OrderNumber, invoiceAddressResult.Value, invoiceEmailAddressResult.Value, invoiceCreditCardNumberResult.Value, testCase.OrderItems);

        // Assert
        orderResult.IsSuccess.Should().BeTrue();
        var order = orderResult.Value;
        order.Should().NotBeNull();
        order.OrderNumber.Should().Be(testCase.OrderNumber);
        order.InvoiceAddress.Value.Should().Be(testCase.AddressValue);
        order.InvoiceEmailAddress.Value.Should().Be(testCase.EmailValue);
        order.InvoiceCreditCardNumber.Value.Should().Be(testCase.CreditCardValue);
        order.OrderItems.Should().HaveCount(testCase.OrderItems.Count);
        order.CreatedAt.Should().Be(default); // CreatedAt is set by EF interceptor, not in factory
    }

    public static TheoryData<InvalidOrderTestCase> InvalidOrderData =>
        [
            new InvalidOrderTestCase(
                "Empty order number should throw exception",
                "",
                "123 Sample Street, 90402 Berlin",
                "customer@example.com",
                "1234-5678-9101-1121",
                new List<OrderItem> { OrderItem.Create("12345", "Gaming Laptop", 1, 1499.99m).Value },
                "Order number cannot be null or empty*"
            ),
            new InvalidOrderTestCase(
                "Null order number should throw exception",
                null,
                "123 Sample Street, 90402 Berlin",
                "customer@example.com",
                "1234-5678-9101-1121",
                new List<OrderItem> { OrderItem.Create("12345", "Gaming Laptop", 1, 1499.99m).Value },
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
    public void Order_ShouldReturnFailure_WhenInvalidDataProvided(InvalidOrderTestCase testCase)
    {
        // Arrange
        var invoiceAddressResult = InvoiceAddress.Create(testCase.AddressValue);
        var invoiceEmailAddressResult = InvoiceEmailAddress.Create(testCase.EmailValue);
        var invoiceCreditCardNumberResult = InvoiceCreditCardNumber.Create(testCase.CreditCardValue);

        // Act
        var orderResult = Order.Create(testCase.OrderNumber!, invoiceAddressResult.Value, invoiceEmailAddressResult.Value, invoiceCreditCardNumberResult.Value, testCase.OrderItems!);

        // Assert
        orderResult.IsFailed.Should().BeTrue();
        orderResult.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void OrderFactory_ShouldCreateValidOrder_WhenValidDataProvided()
    {
        // Arrange
        const string orderNumber = "20250801-FACTORY123";
        const string invoiceAddress = "123 Factory Street, 90402 Berlin";
        const string invoiceEmailAddress = "factory@example.com";
        const string invoiceCreditCardNumber = "1234-5678-9101-1121";
        var orderItems = new[] { ("F123", "Factory Product", 3, 299.99m) };

        // Act
        var result = OrderFactory.Create(
            orderNumber,
            invoiceAddress,
            invoiceEmailAddress,
            invoiceCreditCardNumber,
            orderItems);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var order = result.Value;
        order.Should().NotBeNull();
        order.OrderNumber.Should().Be(orderNumber);
        order.InvoiceAddress.Value.Should().Be(invoiceAddress);
        order.InvoiceEmailAddress.Value.Should().Be(invoiceEmailAddress.ToLowerInvariant());
        order.InvoiceCreditCardNumber.Value.Should().Be(invoiceCreditCardNumber);
        order.OrderItems.Should().HaveCount(1);
        order.OrderItems.First().ProductId.Should().Be("F123");
        order.OrderItems.First().ProductName.Should().Be("Factory Product");
        order.OrderItems.First().ProductAmount.Should().Be(3);
        order.OrderItems.First().ProductPrice.Should().Be(299.99m);
        order.CreatedAt.Should().Be(default); // CreatedAt is set by EF interceptor, not in factory
    }

    [Fact]
    public void OrderTestExtensions_ShouldCreateValidOrderWithDefaults()
    {
        // Act
        var order = OrderTestExtensions.CreateTestOrder();

        // Assert
        order.Should().NotBeNull();
        order.OrderNumber.Should().Be("TEST-001");
        order.InvoiceAddress.Value.Should().Be("123 Test Street, 90210 Test City");
        order.InvoiceEmailAddress.Value.Should().Be("test@example.com");
        order.InvoiceCreditCardNumber.Value.Should().Be("1234-5678-9101-1121");
        order.OrderItems.Should().HaveCount(1);
        order.OrderItems.First().ProductId.Should().Be("TEST-001");
    }

    [Fact]
    public void OrderTestExtensions_ShouldCreateOrderWithCustomItems()
    {
        // Act
        var order = OrderTestExtensions.CreateTestOrder(
            "CUSTOM-123",
            "123 Test Street, 90210 Test City",
            "test@example.com",
            "1234-5678-9101-1121",
            ("ITEM-1", "First Item", 2, 50.0m),
            ("ITEM-2", "Second Item", 1, 75.0m));

        // Assert
        order.OrderNumber.Should().Be("CUSTOM-123");
        order.OrderItems.Should().HaveCount(2);
        order.OrderItems.Should().Contain(i => i.ProductId == "ITEM-1" && i.ProductAmount == 2);
        order.OrderItems.Should().Contain(i => i.ProductId == "ITEM-2" && i.ProductAmount == 1);
    }
}
