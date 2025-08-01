// © 2025 Behrouz Rad. All rights reserved.

using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs;
using OrderService.Application.Enums;
using OrderService.Application.Services;
using OrderService.Domain.Common;
using OrderService.Domain.Entities;
using OrderService.Domain.Repositories;
using OrderService.Domain.ValueObjects;

namespace OrderService.Application.Commands.CreateOrder;

public class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    IStockValidationService stockValidationService,
    ILogger<CreateOrderCommandHandler> logger) : IRequestHandler<CreateOrderCommand, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Creating order with {ProductCount} products", request.OrderDto.Products.Count);

            // Validate stock availability for all products
            foreach (var product in request.OrderDto.Products)
            {
                var isInStock = await stockValidationService.IsProductInStockAsync(
                    product.ProductId,
                    product.ProductAmount,
                    cancellationToken);

                if (!isInStock)
                {
                    logger.LogWarning("Product {ProductId} ({ProductName}) is out of stock. Requested: {RequestedAmount}",
                        product.ProductId, product.ProductName, product.ProductAmount);
                    return Result.Fail(new Error("The product is out of stock")
                                 .WithMetadata("ErrorType", ErrorType.ValidationError));
                }
            }

            var orderNumber = GenerateOrderNumber();

            var invoiceAddress = new InvoiceAddress(request.OrderDto.InvoiceAddress);
            var invoiceEmailAddress = new InvoiceEmailAddress(request.OrderDto.InvoiceEmailAddress);
            var invoiceCreditCardNumber = new InvoiceCreditCardNumber(request.OrderDto.InvoiceCreditCardNumber);

            var orderItems = request.OrderDto.Products.ConvertAll(p =>
                new OrderItem(p.ProductId, p.ProductName, p.ProductAmount, p.ProductPrice));

            var order = new Order(
                orderNumber,
                invoiceAddress,
                invoiceEmailAddress,
                invoiceCreditCardNumber,
                orderItems);

            await orderRepository.AddAsync(order, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Order {OrderNumber} created successfully", order.OrderNumber);

            var orderDto = new OrderDto
            {
                OrderNumber = order.OrderNumber,
                InvoiceAddress = order.InvoiceAddress.Value,
                InvoiceEmailAddress = order.InvoiceEmailAddress.Value,
                InvoiceCreditCardNumber = order.InvoiceCreditCardNumber.Value,
                CreatedAt = order.CreatedAt,
                Products = [.. order.OrderItems.Select(item => new OrderItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductAmount = item.ProductAmount,
                    ProductPrice = item.ProductPrice,
                    TotalPrice = item.TotalPrice
                })]
            };

            return Result.Ok(orderDto);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning("Invalid order data: {Message}", ex.Message);
            return Result.Fail(new Error($"Invalid order data: {ex.Message}")
                         .WithMetadata("ErrorType", ErrorType.ValidationError));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create order");
            return Result.Fail(new Error("Failed to create order. Please try again.")
                         .WithMetadata("ErrorType", ErrorType.InternalError));
        }
    }

    private static string GenerateOrderNumber()
    {
        return DateTime.UtcNow.ToString("yyyyMMdd") + "-" + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
    }
}
