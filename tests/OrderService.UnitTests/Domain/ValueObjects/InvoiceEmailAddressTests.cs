// © 2025 Behrouz Rad. All rights reserved.

using FluentAssertions;
using OrderService.Domain.ValueObjects;

namespace OrderService.UnitTests.Domain.ValueObjects;
public class InvoiceEmailAddressTests
{
    [Theory]
    [InlineData("customer@example.com")]
    [InlineData("test.email@domain.com")]
    [InlineData("user123@test-domain.org")]
    public void InvoiceEmailAddress_ShouldCreateValidEmailAddress_WhenValidEmailProvided(string email)
    {
        // Act
        var invoiceEmail = new InvoiceEmailAddress(email);

        // Assert
        invoiceEmail.Value.Should().Be(email.ToLowerInvariant());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void InvoiceEmailAddress_ShouldThrowArgumentException_WhenEmailIsNullOrEmpty(string email)
    {
        // Act & Assert
        var act = () => new InvoiceEmailAddress(email);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Email address cannot be null or empty*");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@domain.com")]
    [InlineData("user@")]
    [InlineData("user.domain.com")]
    public void InvoiceEmailAddress_ShouldThrowArgumentException_WhenEmailFormatIsInvalid(string email)
    {
        // Act & Assert
        var act = () => new InvoiceEmailAddress(email);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Invalid email format*");
    }
}
