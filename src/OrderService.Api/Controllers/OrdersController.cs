using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Commands.CreateOrder;
using OrderService.Application.Queries.GetOrder;
using OrderService.Application.DTOs;
using OrderService.Application.Enums;

namespace OrderService.Api.Controllers;

public class OrdersController(ISender mediator, ILogger<OrdersController> logger) : OrderBaseController
{
    [HttpPost]
    [ProducesResponseType(typeof(CreateOrderResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createOrderDto, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating order with {ProductCount} products", createOrderDto.Products?.Count ?? 0);

        var command = new CreateOrderCommand(createOrderDto);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation("Order {OrderNumber} created successfully", result.Value.OrderNumber);
            var response = new CreateOrderResponseDto { OrderNumber = result.Value.OrderNumber };
            return CreatedAtAction(nameof(GetOrder), new { orderNumber = result.Value.OrderNumber }, response);
        }

        return (result.Errors[0].Metadata.GetValueOrDefault("ErrorType") as ErrorType?) switch
        {
            ErrorType.ValidationError => BadRequest(result),
            ErrorType.InternalError => InternalServerError(result),
            _ => InternalServerError(result),
        };
    }

    [HttpGet("{orderNumber}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOrder([FromRoute] string orderNumber, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving order {OrderNumber}", orderNumber);

        var query = new GetOrderQuery(orderNumber);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation("Order {OrderNumber} retrieved successfully", orderNumber);
            return Ok(result.Value);
        }

        return (result.Errors[0].Metadata.GetValueOrDefault("ErrorType") as ErrorType?) switch
        {
            ErrorType.NotFound => NotFound(result),
            ErrorType.InternalError => InternalServerError(result),
            _ => InternalServerError(result),
        };
    }
}
