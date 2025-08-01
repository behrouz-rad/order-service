using System.Text.RegularExpressions;

namespace OrderService.Domain.ValueObjects;

public record InvoiceCreditCardNumber
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
        var creditCardRegex = new Regex(@"^\d{13,19}$");

        return creditCardRegex.IsMatch(creditCardNumber);
    }

    public static implicit operator string(InvoiceCreditCardNumber creditCard) => creditCard.Value;
    public static implicit operator InvoiceCreditCardNumber(string creditCard) => new(creditCard);

    public override string ToString() => Value;
}
