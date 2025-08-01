// © 2025 Behrouz Rad. All rights reserved.

namespace OrderService.Application.DTOs;
public record CreateOrderResponseDto
{
    public required string OrderNumber { get; init; }
}
