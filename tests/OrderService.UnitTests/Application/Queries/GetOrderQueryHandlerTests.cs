// © 2025 Behrouz Rad. All rights reserved.

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Application.Enums;
using OrderService.Application.Queries.GetOrder;
using OrderService.Domain.Orders;
using OrderService.Domain.Repositories;
using OrderService.Domain.ValueObjects;

namespace OrderService.UnitTests.Application.Queries;
public class GetOrderQueryHandlerTests : BaseUnitTest
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<ILogger<GetOrderQueryHandler>> _mockLogger;
    private readonly GetOrderQueryHandler _handler;

    public GetOrderQueryHandlerTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockLogger = new Mock<ILogger<GetOrderQueryHandler>>();
        _handler = new GetOrderQueryHandler(_mockOrderRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WithExistingOrder_ShouldReturnOrderDto()
    {
        // Arrange
        const string orderNumber = "20250801-ABC12345";
        var order = CreateTestOrder(orderNumber);

        _mockOrderRepository
            .Setup(x => x.GetByOrderNumberAsync(orderNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var query = new GetOrderQuery(orderNumber);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.OrderNumber.Should().Be(orderNumber);
        result.Value.InvoiceAddress.Should().Be("123 Sample Street, 90402 Berlin");
        result.Value.InvoiceEmailAddress.Should().Be("customer@example.com");
        result.Value.Products.Should().HaveCount(1);
        result.Value.Products.First().ProductId.Should().Be("12345");
    }

    [Fact]
    public async Task Handle_WithNonExistingOrder_ShouldReturnFailureResult()
    {
        // Arrange
        const string orderNumber = "NON-EXISTING";

        _mockOrderRepository
            .Setup(x => x.GetByOrderNumberAsync(orderNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var query = new GetOrderQuery(orderNumber);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Contain("not found");
        result.Errors[0].Metadata.GetValueOrDefault("ErrorType").Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrowsException_ShouldReturnFailureResult()
    {
        // Arrange
        const string orderNumber = "20250731-ABC12345";

        _mockOrderRepository
            .Setup(x => x.GetByOrderNumberAsync(orderNumber, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        var query = new GetOrderQuery(orderNumber);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Contain("Failed to retrieve order");
        result.Errors[0].Metadata.GetValueOrDefault("ErrorType").Should().Be(ErrorType.InternalError);
    }

    private static Order CreateTestOrder(string orderNumber)
    {
        var orderItems = new[] { ("12345", "Gaming Laptop", 2, 1499.99m) };

        return OrderFactory.Create(
            orderNumber,
            "123 Sample Street, 90402 Berlin",
            "customer@example.com",
            "1234-5678-9101-1121",
            orderItems).Value;
    }
}
