// © 2025 Behrouz Rad. All rights reserved.

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Domain.ValueObjects;
using OrderService.Infrastructure.Data;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Tests.Integration.Repositories;

public class OrderRepositoryTests : IDisposable
{
    private OrderDbContext _context;
    private readonly OrderRepository _repository;

    public OrderRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new OrderDbContext(options);
        _repository = new OrderRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnOrder()
    {
        // Arrange
        var order = CreateSampleOrder();
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();

        var orderId = order.Id;

        // Act
        var result = await _repository.GetByIdAsync(orderId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(orderId);
        result.OrderNumber.Should().Be(order.OrderNumber);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(orderId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByOrderNumberAsync_WithExistingOrderNumber_ShouldReturnOrder()
    {
        // Arrange
        const string orderNumber = "ORD-001";
        var order = CreateSampleOrder(orderNumber);
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByOrderNumberAsync(orderNumber);

        // Assert
        result.Should().NotBeNull();
        result!.OrderNumber.Should().Be(orderNumber);
    }

    [Fact]
    public async Task GetByOrderNumberAsync_WithNonExistingOrderNumber_ShouldReturnNull()
    {
        // Arrange
        const string orderNumber = "NON-EXISTING";

        // Act
        var result = await _repository.GetByOrderNumberAsync(orderNumber);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_WithOrdersInDatabase_ShouldReturnAllOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            CreateSampleOrder("ORD-001"),
            CreateSampleOrder("ORD-002"),
            CreateSampleOrder("ORD-003")
        };

        foreach (var order in orders)
        {
            await _context.Orders.AddAsync(order);
        }
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllAsync_WithEmptyDatabase_ShouldReturnEmptyCollection()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task AddAsync_WithValidOrder_ShouldAddOrderToDatabase()
    {
        // Arrange
        var order = CreateSampleOrder("ORD-ADD-001");

        // Act
        var result = await _repository.AddAsync(order);
        await _context.SaveChangesAsync();

        // Assert
        result.Should().Be(order);

        var savedOrder = await _context.Orders.FindAsync(order.Id);
        savedOrder.Should().NotBeNull();
        savedOrder!.OrderNumber.Should().Be("ORD-ADD-001");
    }

    [Fact]
    public async Task UpdateAsync_WithValidOrder_ShouldUpdateOrderInDatabase()
    {
        // Arrange
        var order = CreateSampleOrder("ORD-UPDATE-001");
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();

        _context.Entry(order).State = EntityState.Detached;

        // Act
        var updatedOrder = CreateSampleOrderWithAddress("9 Street, Karlsruhe", "ORD-UPDATE-001");
        typeof(Order).GetProperty("Id")!.SetValue(updatedOrder, order.Id);

        await _repository.UpdateAsync(updatedOrder);
        await _context.SaveChangesAsync();

        // Assert
        var loadedOrder = await _context.Orders.FindAsync(order.Id);
        loadedOrder.Should().NotBeNull();
        loadedOrder!.OrderNumber.Should().Be("ORD-UPDATE-001");
        loadedOrder!.InvoiceAddress.Should().BeEquivalentTo(new InvoiceAddress("9 Street, Karlsruhe"));
    }

    [Fact]
    public async Task DeleteAsync_WithValidOrder_ShouldRemoveOrderFromDatabase()
    {
        // Arrange
        var order = CreateSampleOrder("ORD-DELETE-001");
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(order);
        await _context.SaveChangesAsync();

        // Assert
        var deletedOrder = await _context.Orders.FindAsync(order.Id);
        deletedOrder.Should().BeNull();
    }

    private static Order CreateSampleOrder(string orderNumber = "ORD-001")
    {
        var orderItems = new List<OrderItem>
        {
            new("PROD-001", "Sample Product", 2, 99.99m)
        };

        return new Order(
            orderNumber,
            new InvoiceAddress("123 Sample Street, Berlin"),
            new InvoiceEmailAddress("test@example.com"),
            new InvoiceCreditCardNumber("1234-5678-9101-1121"),
            orderItems
        );
    }

    private static Order CreateSampleOrderWithAddress(string address, string orderNumber = "ORD-001")
    {
        var orderItems = new List<OrderItem>
        {
            new("PROD-001", "Sample Product", 2, 99.99m)
        };

        return new Order(
            orderNumber,
            new InvoiceAddress(address),
            new InvoiceEmailAddress("test@example.com"),
            new InvoiceCreditCardNumber("1234-5678-9101-1121"),
            orderItems
        );
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _context?.Dispose();
        }
    }
}
