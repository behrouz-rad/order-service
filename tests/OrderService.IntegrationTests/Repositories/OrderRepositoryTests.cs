// © 2025 Behrouz Rad. All rights reserved.

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Domain.Orders;
using OrderService.Domain.ValueObjects;
using OrderService.Infrastructure.Data;
using OrderService.Infrastructure.Repositories;

namespace OrderService.IntegrationTests.Repositories;

public class OrderRepositoryTests(OrderWebApplicationFactory factory) : IClassFixture<OrderWebApplicationFactory>
{
    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnOrder()
    {
        // Arrange
        await using var scope = factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        var repository = new OrderRepository(context);

        var order = CreateSampleOrder();
        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();

        var orderId = order.Id;

        // Act
        var result = await repository.GetByIdAsync(orderId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(orderId);
        result.OrderNumber.Should().Be(order.OrderNumber);

        // Cleanup
        await context.Orders.Where(o => o.Id == orderId).ExecuteDeleteAsync();
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        await using var scope = factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        var repository = new OrderRepository(context);

        var orderId = Guid.CreateVersion7();

        // Act
        var result = await repository.GetByIdAsync(orderId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByOrderNumberAsync_WithExistingOrderNumber_ShouldReturnOrder()
    {
        // Arrange
        await using var scope = factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        var repository = new OrderRepository(context);

        var orderNumber = $"ORD-{Guid.CreateVersion7():N}";
        var order = CreateSampleOrder(orderNumber);
        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByOrderNumberAsync(orderNumber);

        // Assert
        result.Should().NotBeNull();
        result!.OrderNumber.Should().Be(orderNumber);

        // Cleanup
        await context.Orders.Where(o => o.Id == order.Id).ExecuteDeleteAsync();
    }

    [Fact]
    public async Task GetByOrderNumberAsync_WithNonExistingOrderNumber_ShouldReturnNull()
    {
        // Arrange
        await using var scope = factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        var repository = new OrderRepository(context);

        var orderNumber = $"NON-EXISTING-{Guid.CreateVersion7():N}";

        // Act
        var result = await repository.GetByOrderNumberAsync(orderNumber);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_WithOrdersInDatabase_ShouldReturnAllOrders()
    {
        // Arrange
        await using var scope = factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        var repository = new OrderRepository(context);

        var testId = Guid.CreateVersion7().ToString("N")[..8];
        var orders = new List<Order>
        {
            CreateSampleOrder($"ORD-{testId}-001"),
            CreateSampleOrder($"ORD-{testId}-002"),
            CreateSampleOrder($"ORD-{testId}-003")
        };

        foreach (var order in orders)
        {
            await context.Orders.AddAsync(order);
        }
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        var testOrders = result.Where(o => o.OrderNumber.Contains(testId)).ToList();
        testOrders.Should().HaveCount(3);

        // Cleanup
        var orderIds = orders.Select(o => o.Id).ToList();
        await context.Orders.Where(o => orderIds.Contains(o.Id)).ExecuteDeleteAsync();
    }

    [Fact]
    public async Task AddAsync_WithValidOrder_ShouldAddOrderToDatabase()
    {
        // Arrange
        await using var scope = factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        var repository = new OrderRepository(context);

        var orderNumber = $"ORD-ADD-{Guid.CreateVersion7():N}";
        var order = CreateSampleOrder(orderNumber);

        // Act
        var result = await repository.AddAsync(order);
        await context.SaveChangesAsync();

        // Assert
        result.Should().Be(order);

        var savedOrder = await context.Orders.FindAsync(order.Id);
        savedOrder.Should().NotBeNull();
        savedOrder!.OrderNumber.Should().Be(orderNumber);

        // Cleanup
        await context.Orders.Where(o => o.Id == order.Id).ExecuteDeleteAsync();
    }

    [Fact]
    public async Task UpdateAsync_WithValidOrder_ShouldUpdateOrderInDatabase()
    {
        // Arrange
        await using var scope = factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        var repository = new OrderRepository(context);

        var orderNumber = $"ORD-UPDATE-{Guid.CreateVersion7():N}";
        var order = CreateSampleOrder(orderNumber);
        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();

        context.Entry(order).State = EntityState.Detached;

        // Act
        var updatedOrder = CreateSampleOrderWithAddress("9 Street, Karlsruhe", orderNumber);
        typeof(Order).GetProperty("Id")!.SetValue(updatedOrder, order.Id);

        await repository.UpdateAsync(updatedOrder);
        await context.SaveChangesAsync();

        // Assert
        var loadedOrder = await context.Orders.FindAsync(order.Id);
        loadedOrder.Should().NotBeNull();
        loadedOrder!.OrderNumber.Should().Be(orderNumber);
        loadedOrder!.InvoiceAddress.Should().BeEquivalentTo(new InvoiceAddress("9 Street, Karlsruhe"));

        // Cleanup
        await context.Orders.Where(o => o.Id == order.Id).ExecuteDeleteAsync();
    }

    [Fact]
    public async Task DeleteAsync_WithValidOrder_ShouldRemoveOrderFromDatabase()
    {
        // Arrange
        await using var scope = factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        var repository = new OrderRepository(context);

        var orderNumber = $"ORD-DELETE-{Guid.CreateVersion7():N}";
        var order = CreateSampleOrder(orderNumber);
        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(order);
        await context.SaveChangesAsync();

        // Assert
        var deletedOrder = await context.Orders.FindAsync(order.Id);
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
}
