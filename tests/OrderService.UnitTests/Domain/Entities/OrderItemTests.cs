// © 2025 Behrouz Rad. All rights reserved.

using FluentAssertions;
using OrderService.Domain.ValueObjects;

namespace OrderService.UnitTests.Domain.Entities;
public class OrderItemTests
{
    [Fact]
    public void OrderItem_ShouldCreateValidOrderItem_WhenValidDataProvided()
    {
        // Arrange
        const string productId = "12345";
        const string productName = "Gaming Laptop";
        const int productAmount = 2;
        const decimal productPrice = 1499.99m;

        // Act
        var orderItem = new OrderItem(productId, productName, productAmount, productPrice);

        // Assert
        orderItem.ProductId.Should().Be(productId);
        orderItem.ProductName.Should().Be(productName);
        orderItem.ProductAmount.Should().Be(productAmount);
        orderItem.ProductPrice.Should().Be(productPrice);
        orderItem.TotalPrice.Should().Be(2999.98m);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void OrderItem_ShouldThrowArgumentException_WhenProductIdIsNullOrEmpty(string productId)
    {
        // Act & Assert
        Func<OrderItem> act = () => new OrderItem(productId, "Product", 1, 10.0m);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Product ID cannot be null or empty*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void OrderItem_ShouldThrowArgumentException_WhenProductNameIsNullOrEmpty(string productName)
    {
        // Act & Assert
        Func<OrderItem> act = () => new OrderItem("123", productName, 1, 10.0m);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Product name cannot be null or empty*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void OrderItem_ShouldThrowArgumentException_WhenProductAmountIsZeroOrNegative(int productAmount)
    {
        // Act & Assert
        Func<OrderItem> act = () => new OrderItem("123", "Product", productAmount, 10.0m);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Product amount must be greater than zero*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void OrderItem_ShouldThrowArgumentException_WhenProductPriceIsZeroOrNegative(decimal productPrice)
    {
        // Act & Assert
        Func<OrderItem> act = () => new OrderItem("123", "Product", 1, productPrice);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Product price must be greater than zero*");
    }
}
