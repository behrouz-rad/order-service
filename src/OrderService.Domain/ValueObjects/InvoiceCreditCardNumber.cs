// © 2025 Behrouz Rad. All rights reserved.

using System.Text.RegularExpressions;

namespace OrderService.Domain.ValueObjects;

public partial record InvoiceCreditCardNumber
{
    public string Value { get; init; }

    public InvoiceCreditCardNumber(string creditCardNumber)
    {
        if (string.IsNullOrWhiteSpace(creditCardNumber))
        {
            throw new ArgumentException("Credit card number cannot be null or empty", nameof(creditCardNumber));
        }

        var cleanNumber = creditCardNumber.Replace("-", "").Replace(" ", "");

        if (!IsValidCreditCardNumber(cleanNumber))
        {
            throw new ArgumentException("Invalid credit card number format", nameof(creditCardNumber));
        }

        Value = creditCardNumber;
    }

    private static bool IsValidCreditCardNumber(string creditCardNumber)
    {
        var creditCardRegex = CreditCardRegex();

        return creditCardRegex.IsMatch(creditCardNumber);
    }

    public static implicit operator string(InvoiceCreditCardNumber creditCard) => creditCard.Value;
    public static implicit operator InvoiceCreditCardNumber(string creditCard) => new(creditCard);

    public override string ToString() => Value;

    [GeneratedRegex(@"^\d{13,19}$", RegexOptions.Compiled, matchTimeoutMilliseconds: 1000)]
    private static partial Regex CreditCardRegex();
}
