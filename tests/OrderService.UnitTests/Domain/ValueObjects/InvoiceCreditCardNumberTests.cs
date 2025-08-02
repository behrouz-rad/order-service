// © 2025 Behrouz Rad. All rights reserved.

using FluentAssertions;
using OrderService.Domain.ValueObjects;

namespace OrderService.UnitTests.Domain.ValueObjects;
public class InvoiceCreditCardNumberTests
{
    [Theory]
    [InlineData("1234-5678-9101-1121")]
    [InlineData("1234567891011121")]
    [InlineData("4000 0000 0000 0002")]
    public void InvoiceCreditCardNumber_ShouldCreateValidCreditCard_WhenValidNumberProvided(string creditCardNumber)
    {
        // Act
        var invoiceCreditCard = new InvoiceCreditCardNumber(creditCardNumber);

        // Assert
        invoiceCreditCard.Value.Should().Be(creditCardNumber);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void InvoiceCreditCardNumber_ShouldThrowArgumentException_WhenNumberIsNullOrEmpty(string creditCardNumber)
    {
        // Act & Assert
        var act = () => new InvoiceCreditCardNumber(creditCardNumber);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Credit card number cannot be null or empty*");
    }

    [Theory]
    [InlineData("123")]
    [InlineData("abcd-efgh-ijkl-mnop")]
    [InlineData("12345")]
    public void InvoiceCreditCardNumber_ShouldThrowArgumentException_WhenNumberFormatIsInvalid(string creditCardNumber)
    {
        // Act & Assert
        var act = () => new InvoiceCreditCardNumber(creditCardNumber);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Invalid credit card number format*");
    }
}
