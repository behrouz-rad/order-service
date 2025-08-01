using MediatR;
using FluentResults;
using OrderService.Application.DTOs;
using OrderService.Application.Enums;
using OrderService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace OrderService.Application.Queries.GetOrder;

public class GetOrderQueryHandler(IOrderRepository orderRepository, ILogger<GetOrderQueryHandler> logger) : IRequestHandler<GetOrderQuery, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Retrieving order with number {OrderNumber}", request.OrderNumber);

            var order = await orderRepository.GetByOrderNumberAsync(request.OrderNumber, cancellationToken);
            if (order is null)
            {
                logger.LogWarning("Order {OrderNumber} not found", request.OrderNumber);
                return Result.Fail(new Error($"Order with number {request.OrderNumber} not found")
                             .WithMetadata("ErrorType", ErrorType.NotFound));
            }

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

            logger.LogInformation("Order {OrderNumber} retrieved successfully", request.OrderNumber);
            return Result.Ok(orderDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve order {OrderNumber}", request.OrderNumber);
            return Result.Fail(new Error($"Failed to retrieve order {request.OrderNumber}")
                         .WithMetadata("ErrorType", ErrorType.InternalError));
        }
    }
}
