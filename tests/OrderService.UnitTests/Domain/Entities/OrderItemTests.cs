// © 2025 Behrouz Rad. All rights reserved.

using FluentAssertions;
using OrderService.Domain.Orders;

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
        var result = OrderItem.Create(productId, productName, productAmount, productPrice);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var orderItem = result.Value;
        orderItem.ProductId.Should().Be(productId);
        orderItem.ProductName.Should().Be(productName);
        orderItem.ProductAmount.Should().Be(productAmount);
        orderItem.ProductPrice.Should().Be(productPrice);
    }

    [Fact]
    public void OrderItem_ShouldReturnFailure_WhenProductIdIsNull()
    {
        // Act
        var result = OrderItem.Create(null!, "Product", 1, 10.0m);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Product ID cannot be null or empty");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void OrderItem_ShouldReturnFailure_WhenProductIdIsNullOrEmpty(string? productId)
    {
        // Act
        var result = OrderItem.Create(productId!, "Product", 1, 10.0m);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Product ID cannot be null or empty");
    }

    [Fact]
    public void OrderItem_ShouldReturnFailure_WhenProductNameIsNull()
    {
        // Act
        var result = OrderItem.Create("123", null!, 1, 10.0m);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Product name cannot be null or empty");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void OrderItem_ShouldReturnFailure_WhenProductNameIsNullOrEmpty(string? productName)
    {
        // Act
        var result = OrderItem.Create("123", productName!, 1, 10.0m);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Product name cannot be null or empty");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void OrderItem_ShouldReturnFailure_WhenProductAmountIsZeroOrNegative(int productAmount)
    {
        // Act
        var result = OrderItem.Create("123", "Product", productAmount, 10.0m);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Product amount must be greater than zero");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void OrderItem_ShouldReturnFailure_WhenProductPriceIsZeroOrNegative(decimal productPrice)
    {
        // Act
        var result = OrderItem.Create("123", "Product", 1, productPrice);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Product price must be greater than zero");
    }
}
