// © 2025 Behrouz Rad. All rights reserved.

namespace OrderService.Domain.Orders;

public record InvoiceAddress
{
    public string Value { get; init; }

    public InvoiceAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("Address cannot be null or empty", nameof(address));
        }

        if (address.Length < 10)
        {
            throw new ArgumentException("Address must be at least 10 characters long", nameof(address));
        }

        Value = address.Trim();
    }

    public static implicit operator string(InvoiceAddress address) => address.Value;
    public static implicit operator InvoiceAddress(string address) => new(address);

    public override string ToString() => Value;
}
