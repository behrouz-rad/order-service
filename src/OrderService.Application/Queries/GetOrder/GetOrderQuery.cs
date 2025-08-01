// © 2025 Behrouz Rad. All rights reserved.

using FluentResults;
using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Queries.GetOrder;

public record GetOrderQuery(string OrderNumber) : IRequest<Result<OrderDto>>;
