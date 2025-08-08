// © 2025 Behrouz Rad. All rights reserved.

using System.Collections.ObjectModel;

namespace OrderService.Application.DTOs;

public record OrderDto
{
    public required string OrderNumber { get; init; }
    public required IReadOnlyCollection<OrderItemDto> Products { get; init; }
    public required string InvoiceAddress { get; init; }
    public required string InvoiceEmailAddress { get; init; }
    public required string InvoiceCreditCardNumber { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}

public record OrderItemDto
{
    public required string ProductId { get; init; }
    public required string ProductName { get; init; }
    public required int ProductAmount { get; init; }
    public required decimal ProductPrice { get; init; }
}
