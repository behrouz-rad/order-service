// © 2025 Behrouz Rad. All rights reserved.

using System.Text.RegularExpressions;
using FluentResults;

namespace OrderService.Domain.ValueObjects;

public partial record InvoiceCreditCardNumber
{
    public string Value { get; init; } = string.Empty;

    private InvoiceCreditCardNumber() { }

    private InvoiceCreditCardNumber(string creditCardNumber)
    {
        Value = creditCardNumber;
    }

    public static Result<InvoiceCreditCardNumber> Create(string creditCardNumber)
    {
        if (string.IsNullOrWhiteSpace(creditCardNumber))
        {
            return Result.Fail(new Error("Credit card number cannot be null or empty"));
        }

        var cleanNumber = creditCardNumber.Replace("-", "").Replace(" ", "");

        if (!IsValidCreditCardNumber(cleanNumber))
        {
            return Result.Fail(new Error("Invalid credit card number format"));
        }

        return Result.Ok(new InvoiceCreditCardNumber(creditCardNumber));
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
