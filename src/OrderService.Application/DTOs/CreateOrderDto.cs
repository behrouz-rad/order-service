// © 2025 Behrouz Rad. All rights reserved.

namespace OrderService.Application.DTOs;

public record CreateOrderDto
{
    public required List<CreateOrderItemDto> Products { get; init; }
    public required string InvoiceAddress { get; init; }
    public required string InvoiceEmailAddress { get; init; }
    public required string InvoiceCreditCardNumber { get; init; }
}

public record CreateOrderItemDto
{
    public required string ProductId { get; init; }
    public required string ProductName { get; init; }
    public required int ProductAmount { get; init; }
    public required decimal ProductPrice { get; init; }
}
