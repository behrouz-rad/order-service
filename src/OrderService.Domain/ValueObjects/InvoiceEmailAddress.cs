using System.Text.RegularExpressions;

namespace OrderService.Domain.ValueObjects;

public record InvoiceEmailAddress
{
    public string Value { get; init; }

    public InvoiceEmailAddress(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email address cannot be null or empty", nameof(email));
        }

        if (!IsValidEmail(email))
        {
            throw new ArgumentException("Invalid email format", nameof(email));
        }

        Value = email.ToLowerInvariant();
    }

    private static bool IsValidEmail(string email)
    {
        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
        return emailRegex.IsMatch(email);
    }

    public static implicit operator string(InvoiceEmailAddress email) => email.Value;
    public static implicit operator InvoiceEmailAddress(string email) => new(email);

    public override string ToString() => Value;
}
