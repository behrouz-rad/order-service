// © 2025 Behrouz Rad. All rights reserved.

using System.Globalization;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs;
using OrderService.Application.Enums;
using OrderService.Application.Persistence;
using OrderService.Application.Services;
using OrderService.Domain.Orders;
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

            var invoiceAddressResult = InvoiceAddress.Create(request.OrderDto.InvoiceAddress);
            if (invoiceAddressResult.IsFailed)
            {
                logger.LogWarning("Invalid invoice address: {Errors}", string.Join(", ", invoiceAddressResult.Errors.Select(e => e.Message)));
                return Result.Fail(new Error($"Invalid order data: {string.Join(", ", invoiceAddressResult.Errors.Select(e => e.Message))}")
                             .WithMetadata("ErrorType", ErrorType.ValidationError));
            }

            var invoiceEmailAddressResult = InvoiceEmailAddress.Create(request.OrderDto.InvoiceEmailAddress);
            if (invoiceEmailAddressResult.IsFailed)
            {
                logger.LogWarning("Invalid invoice email address: {Errors}", string.Join(", ", invoiceEmailAddressResult.Errors.Select(e => e.Message)));
                return Result.Fail(new Error($"Invalid order data: {string.Join(", ", invoiceEmailAddressResult.Errors.Select(e => e.Message))}")
                             .WithMetadata("ErrorType", ErrorType.ValidationError));
            }

            var invoiceCreditCardNumberResult = InvoiceCreditCardNumber.Create(request.OrderDto.InvoiceCreditCardNumber);
            if (invoiceCreditCardNumberResult.IsFailed)
            {
                logger.LogWarning("Invalid invoice credit card number: {Errors}", string.Join(", ", invoiceCreditCardNumberResult.Errors.Select(e => e.Message)));
                return Result.Fail(new Error($"Invalid order data: {string.Join(", ", invoiceCreditCardNumberResult.Errors.Select(e => e.Message))}")
                             .WithMetadata("ErrorType", ErrorType.ValidationError));
            }

            var orderItems = new List<OrderItem>();
            foreach (var product in request.OrderDto.Products)
            {
                var orderItemResult = OrderItem.Create(product.ProductId, product.ProductName, product.ProductAmount, product.ProductPrice);
                if (orderItemResult.IsFailed)
                {
                    logger.LogWarning("Invalid order item: {Errors}", string.Join(", ", orderItemResult.Errors.Select(e => e.Message)));
                    return Result.Fail(new Error($"Invalid order data: {string.Join(", ", orderItemResult.Errors.Select(e => e.Message))}")
                                 .WithMetadata("ErrorType", ErrorType.ValidationError));
                }
                orderItems.Add(orderItemResult.Value);
            }

            var orderResult = Order.Create(
                orderNumber,
                invoiceAddressResult.Value,
                invoiceEmailAddressResult.Value,
                invoiceCreditCardNumberResult.Value,
                orderItems);

            if (orderResult.IsFailed)
            {
                logger.LogWarning("Invalid order: {Errors}", string.Join(", ", orderResult.Errors.Select(e => e.Message)));
                return Result.Fail(new Error($"Invalid order data: {string.Join(", ", orderResult.Errors.Select(e => e.Message))}")
                             .WithMetadata("ErrorType", ErrorType.ValidationError));
            }

            var order = orderResult.Value;

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
                    ProductPrice = item.ProductPrice
                })]
            };

            return Result.Ok(orderDto);
        }
        catch (ArgumentException ex)
        {
            logger.LogError(ex, "Invalid order data");
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
        return DateTime.UtcNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + "-" + Guid.CreateVersion7().ToString("N").ToUpperInvariant();
    }
}
