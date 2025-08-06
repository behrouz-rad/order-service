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
        var result = InvoiceCreditCardNumber.Create(creditCardNumber);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(creditCardNumber);
    }

    [Fact]
    public void InvoiceCreditCardNumber_ShouldReturnFailure_WhenNumberIsNull()
    {
        // Act
        var result = InvoiceCreditCardNumber.Create(null!);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Credit card number cannot be null or empty");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void InvoiceCreditCardNumber_ShouldReturnFailure_WhenNumberIsNullOrEmpty(string? creditCardNumber)
    {
        // Act
        var result = InvoiceCreditCardNumber.Create(creditCardNumber!);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Credit card number cannot be null or empty");
    }

    [Theory]
    [InlineData("123")]
    [InlineData("abcd-efgh-ijkl-mnop")]
    [InlineData("12345")]
    public void InvoiceCreditCardNumber_ShouldReturnFailure_WhenNumberFormatIsInvalid(string creditCardNumber)
    {
        // Act
        var result = InvoiceCreditCardNumber.Create(creditCardNumber);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Invalid credit card number format");
    }
}
