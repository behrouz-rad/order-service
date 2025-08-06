// © 2025 Behrouz Rad. All rights reserved.

using FluentResults;

namespace OrderService.Domain.Orders;

public record InvoiceAddress
{
    public string Value { get; init; } = string.Empty;

    private InvoiceAddress() { }

    private InvoiceAddress(string address)
    {
        Value = address.Trim();
    }

    public static Result<InvoiceAddress> Create(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            return Result.Fail(new Error("Address cannot be null or empty"));
        }

        if (address.Length < 10)
        {
            return Result.Fail(new Error("Address must be at least 10 characters long"));
        }

        return Result.Ok(new InvoiceAddress(address));
    }

    public static implicit operator string(InvoiceAddress address) => address.Value;
    public static implicit operator InvoiceAddress(string address) => new(address);

    public override string ToString() => Value;
}
