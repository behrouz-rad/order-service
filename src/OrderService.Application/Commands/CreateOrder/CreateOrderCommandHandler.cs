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

namespace OrderService.Application.Commands.CreateOrder;

public class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    IStockValidationService stockValidationService,
    ILogger<CreateOrderCommandHandler> logger) : IRequestHandler<CreateOrderCommand, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
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

        var orderItems = request.OrderDto.Products.Select(p =>
            (p.ProductId, p.ProductName, p.ProductAmount, p.ProductPrice));

        var orderResult = OrderFactory.Create(
            orderNumber,
            request.OrderDto.InvoiceAddress,
            request.OrderDto.InvoiceEmailAddress,
            request.OrderDto.InvoiceCreditCardNumber,
            orderItems);

        if (orderResult.IsFailed)
        {
            logger.LogWarning("Order validation failed with {ErrorCount} errors: {Errors}",
                orderResult.Errors.Count,
                string.Join(", ", orderResult.Errors.Select(e => e.Message)));

            var validationErrors = orderResult.Errors.Select(e =>
                new Error(e.Message).WithMetadata("ErrorType", ErrorType.ValidationError));

            return Result.Fail(validationErrors);
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

    private static string GenerateOrderNumber()
    {
        return DateTime.UtcNow.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + "-" + Guid.CreateVersion7().ToString("N").ToUpperInvariant();
    }
}
