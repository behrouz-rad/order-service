// © 2025 Behrouz Rad. All rights reserved.

using FluentResults;
using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Commands.CreateOrder;

public record CreateOrderCommand(CreateOrderDto OrderDto) : IRequest<Result<OrderDto>>;
