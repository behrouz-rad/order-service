// © 2025 Behrouz Rad. All rights reserved.

using FluentAssertions;
using Mapster;
using OrderService.Application.DTOs;
using OrderService.Domain.Orders;

namespace OrderService.UnitTests.Application.Mappings;

public class MapsterConfigurationTests : BaseUnitTest
{
    [Fact]
    public void OrderToOrderDto_ShouldMapCorrectly()
    {
        // Arrange
        var orderItems = new[] { ("12345", "Gaming Laptop", 2, 1499.99m) };
        var order = OrderFactory.Create(
            "20250801-ABC12345",
            "123 Sample Street, 90402 Berlin",
            "customer@example.com",
            "1234-5678-9101-1121",
            orderItems).Value;

        // Act
        var orderDto = order.Adapt<OrderDto>();

        // Assert
        orderDto.Should().NotBeNull();
        orderDto.OrderNumber.Should().Be("20250801-ABC12345");
        orderDto.InvoiceAddress.Should().Be("123 Sample Street, 90402 Berlin");
        orderDto.InvoiceEmailAddress.Should().Be("customer@example.com");
        orderDto.InvoiceCreditCardNumber.Should().Be("1234-5678-9101-1121");
        orderDto.Products.Should().NotBeNull();
        orderDto.Products.Should().HaveCount(1);
        orderDto.Products.First().ProductId.Should().Be("12345");
        orderDto.Products.First().ProductName.Should().Be("Gaming Laptop");
        orderDto.Products.First().ProductAmount.Should().Be(2);
        orderDto.Products.First().ProductPrice.Should().Be(1499.99m);
    }
}
