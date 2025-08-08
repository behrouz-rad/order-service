// © 2025 Behrouz Rad. All rights reserved.

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrderService.Application.Commands.CreateOrder;
using OrderService.Application.DTOs;
using OrderService.Application.Enums;
using OrderService.Application.Persistence;
using OrderService.Application.Services;
using OrderService.Domain.Orders;
using OrderService.Domain.Repositories;

namespace OrderService.UnitTests.Application.Commands;
public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<IStockValidationService> _mockStockValidationService;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILogger<CreateOrderCommandHandler>> _mockLogger;
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockStockValidationService = new Mock<IStockValidationService>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogger = new Mock<ILogger<CreateOrderCommandHandler>>();
        _handler = new CreateOrderCommandHandler(_mockOrderRepository.Object, _mockUnitOfWork.Object, _mockStockValidationService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateOrderAndReturnOrderNumber()
    {
        // Arrange
        var createOrderDto = new CreateOrderDto
        {
            Products = new List<CreateOrderItemDto>
            {
                new() { ProductId = "12345", ProductName = "Gaming Laptop", ProductAmount = 2, ProductPrice = 1499.99m }
            },
            InvoiceAddress = "123 Sample Street, 90402 Berlin",
            InvoiceEmailAddress = "customer@example.com",
            InvoiceCreditCardNumber = "1234-5678-9101-1121"
        };

        var command = new CreateOrderCommand(createOrderDto);

        _mockOrderRepository
            .Setup(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order order, CancellationToken _) => order);

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mockStockValidationService
            .Setup(x => x.IsProductInStockAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.OrderNumber.Should().StartWith(DateTime.UtcNow.ToString("yyyyMMdd"));
        result.Value.InvoiceAddress.Should().Be("123 Sample Street, 90402 Berlin");
        result.Value.InvoiceEmailAddress.Should().Be("customer@example.com");
        result.Value.InvoiceCreditCardNumber.Should().Be("1234-5678-9101-1121");
        result.Value.Products.Should().HaveCount(1);
        result.Value.Products[0].ProductId.Should().Be("12345");
        result.Value.Products[0].ProductName.Should().Be("Gaming Laptop");

        _mockOrderRepository.Verify(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidEmail_ShouldReturnFailureResult()
    {
        // Arrange
        var createOrderDto = new CreateOrderDto
        {
            Products = new List<CreateOrderItemDto>
            {
                new() { ProductId = "12345", ProductName = "Gaming Laptop", ProductAmount = 2, ProductPrice = 1499.99m }
            },
            InvoiceAddress = "123 Sample Street, 90402 Berlin",
            InvoiceEmailAddress = "invalid-email",
            InvoiceCreditCardNumber = "1234-5678-9101-1121"
        };

        var command = new CreateOrderCommand(createOrderDto);

        _mockStockValidationService
            .Setup(x => x.IsProductInStockAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Contain("Invalid email format");
        result.Errors[0].Metadata.GetValueOrDefault("ErrorType").Should().Be(ErrorType.ValidationError);
    }

    [Fact]
    public async Task Handle_WhenProductIsOutOfStock_ShouldReturnFailureResult()
    {
        // Arrange
        var createOrderDto = new CreateOrderDto
        {
            Products = new List<CreateOrderItemDto>
            {
                new() { ProductId = "12345", ProductName = "Gaming Laptop", ProductAmount = 2, ProductPrice = 1499.99m }
            },
            InvoiceAddress = "123 Sample Street, 90402 Berlin",
            InvoiceEmailAddress = "customer@example.com",
            InvoiceCreditCardNumber = "1234-5678-9101-1121"
        };

        var command = new CreateOrderCommand(createOrderDto);

        _mockStockValidationService
            .Setup(x => x.IsProductInStockAsync("12345", 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Be("The product is out of stock");
        result.Errors[0].Metadata.GetValueOrDefault("ErrorType").Should().Be(ErrorType.ValidationError);

        _mockStockValidationService.Verify(x => x.IsProductInStockAsync("12345", 2, It.IsAny<CancellationToken>()), Times.Once);
        _mockOrderRepository.Verify(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
