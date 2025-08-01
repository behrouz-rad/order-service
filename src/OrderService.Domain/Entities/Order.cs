using OrderService.Domain.ValueObjects;

namespace OrderService.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public string OrderNumber { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public InvoiceAddress InvoiceAddress { get; private set; } = null!;
    public InvoiceEmailAddress InvoiceEmailAddress { get; private set; } = null!;
    public InvoiceCreditCardNumber InvoiceCreditCardNumber { get; private set; } = null!;

    private readonly List<OrderItem> _orderItems = [];
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    public decimal TotalAmount => _orderItems.Sum(item => item.TotalPrice);

    private Order() { }

    public Order(
        string orderNumber,
        InvoiceAddress invoiceAddress,
        InvoiceEmailAddress invoiceEmailAddress,
        InvoiceCreditCardNumber invoiceCreditCardNumber,
        IEnumerable<OrderItem> orderItems)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
        {
            throw new ArgumentException("Order number cannot be null or empty", nameof(orderNumber));
        }

        if (!orderItems.Any())
        {
            throw new ArgumentException("Order must contain at least one item", nameof(orderItems));
        }

        Id = Guid.NewGuid();
        OrderNumber = orderNumber;
        CreatedAt = DateTime.UtcNow;
        InvoiceAddress = invoiceAddress ?? throw new ArgumentNullException(nameof(invoiceAddress));
        InvoiceEmailAddress = invoiceEmailAddress ?? throw new ArgumentNullException(nameof(invoiceEmailAddress));
        InvoiceCreditCardNumber = invoiceCreditCardNumber ?? throw new ArgumentNullException(nameof(invoiceCreditCardNumber));

        _orderItems.AddRange(orderItems);
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
