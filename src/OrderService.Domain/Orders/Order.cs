// © 2025 Behrouz Rad. All rights reserved.

using FluentResults;
using OrderService.Domain.ValueObjects;

namespace OrderService.Domain.Orders;

public class Order
{
    public Guid Id { get; private set; }
    public string OrderNumber { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public InvoiceAddress InvoiceAddress { get; private set; } = null!;
    public InvoiceEmailAddress InvoiceEmailAddress { get; private set; } = null!;
    public InvoiceCreditCardNumber InvoiceCreditCardNumber { get; private set; } = null!;

    private readonly List<OrderItem> _orderItems = [];
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    private Order() { }

    private Order(
        string orderNumber,
        InvoiceAddress invoiceAddress,
        InvoiceEmailAddress invoiceEmailAddress,
        InvoiceCreditCardNumber invoiceCreditCardNumber,
        IEnumerable<OrderItem> orderItems)
    {
        Id = Guid.CreateVersion7();
        OrderNumber = orderNumber;
        InvoiceAddress = invoiceAddress;
        InvoiceEmailAddress = invoiceEmailAddress;
        InvoiceCreditCardNumber = invoiceCreditCardNumber;

        _orderItems.AddRange(orderItems);
    }

    public static Result<Order> Create(
        string orderNumber,
        InvoiceAddress invoiceAddress,
        InvoiceEmailAddress invoiceEmailAddress,
        InvoiceCreditCardNumber invoiceCreditCardNumber,
        IEnumerable<OrderItem> orderItems)
    {
        var errors = new List<IError>();

        if (string.IsNullOrWhiteSpace(orderNumber))
        {
            errors.Add(new Error("Order number cannot be null or empty"));
        }

        if (orderItems?.Any() is not true)
        {
            errors.Add(new Error("Order must contain at least one item"));
        }

        if (invoiceAddress is null)
        {
            errors.Add(new Error("Invoice address is required"));
        }

        if (invoiceEmailAddress is null)
        {
            errors.Add(new Error("Invoice email address is required"));
        }

        if (invoiceCreditCardNumber is null)
        {
            errors.Add(new Error("Invoice credit card number is required"));
        }

        if (errors.Count != 0)
        {
            return Result.Fail(errors);
        }

        return Result.Ok(new Order(orderNumber, invoiceAddress!, invoiceEmailAddress!, invoiceCreditCardNumber!, orderItems!));
    }

    internal void SetCreatedAt(DateTimeOffset createdAt)
    {
        CreatedAt = createdAt;
    }

    public void AddOrderItem(OrderItem orderItem)
    {
        ArgumentNullException.ThrowIfNull(orderItem);

        _orderItems.Add(orderItem);
    }

    public void RemoveOrderItem(OrderItem orderItem)
    {
        ArgumentNullException.ThrowIfNull(orderItem);

        _orderItems.Remove(orderItem);
    }
}
