// © 2025 Behrouz Rad. All rights reserved.

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using OrderService.Application.DTOs;

namespace OrderService.Tests.Integration.Controllers;
public class OrdersControllerTests(OrderWebApplicationFactory factory) : IClassFixture<OrderWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateOrder_WithValidData_ShouldReturnCreatedResult()
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

        // Act
        var response = await _client.PostAsJsonAsync("/api/orders", createOrderDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("orderNumber");
    }

    [Fact]
    public async Task CreateOrder_WithInvalidEmail_ShouldReturnBadRequest()
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

        // Act
        var response = await _client.PostAsJsonAsync("/api/orders", createOrderDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetOrder_WithExistingOrder_ShouldReturnOrder()
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

        var createResponse = await _client.PostAsJsonAsync("/api/orders", createOrderDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<dynamic>();
        var orderNumber = createResult?.GetProperty("orderNumber").GetString();

        // Act
        var response = await _client.GetAsync($"/api/orders/{orderNumber}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var orderDto = await response.Content.ReadFromJsonAsync<OrderDto>();
        orderDto.Should().NotBeNull();
        orderDto!.OrderNumber.Should().Be(orderNumber);
        orderDto.InvoiceAddress.Should().Be("123 Sample Street, 90402 Berlin");
        orderDto.InvoiceEmailAddress.Should().Be("customer@example.com");
        orderDto.Products.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetOrder_WithNonExistingOrder_ShouldReturnNotFound()
    {
        // Arrange
        const string nonExistingOrderNumber = "NON-EXISTING-ORDER";

        // Act
        var response = await _client.GetAsync($"/api/orders/{nonExistingOrderNumber}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
