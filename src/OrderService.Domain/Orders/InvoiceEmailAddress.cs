// © 2025 Behrouz Rad. All rights reserved.

using System.Text.RegularExpressions;
using FluentResults;

namespace OrderService.Domain.ValueObjects;

public partial record InvoiceEmailAddress
{
    public string Value { get; init; } = string.Empty;

    private InvoiceEmailAddress() { }

    private InvoiceEmailAddress(string email)
    {
        Value = email.ToLowerInvariant();
    }

    public static Result<InvoiceEmailAddress> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result.Fail(new Error("Email address cannot be null or empty"));
        }

        if (!IsValidEmail(email))
        {
            return Result.Fail(new Error("Invalid email format"));
        }

        return Result.Ok(new InvoiceEmailAddress(email));
    }

    private static bool IsValidEmail(string email)
    {
        var emailRegex = EmailRegex();
        return emailRegex.IsMatch(email);
    }

    public static implicit operator string(InvoiceEmailAddress email) => email.Value;
    public static implicit operator InvoiceEmailAddress(string email) => new(email);

    public override string ToString() => Value;
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
    private static partial Regex EmailRegex();
}
