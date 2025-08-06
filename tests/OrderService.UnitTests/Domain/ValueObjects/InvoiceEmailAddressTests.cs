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
        var result = InvoiceEmailAddress.Create(email);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(email.ToLowerInvariant());
    }

    [Fact]
    public void InvoiceEmailAddress_ShouldReturnFailure_WhenEmailIsNull()
    {
        // Act
        var result = InvoiceEmailAddress.Create(null!);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Email address cannot be null or empty");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void InvoiceEmailAddress_ShouldReturnFailure_WhenEmailIsNullOrEmpty(string? email)
    {
        // Act
        var result = InvoiceEmailAddress.Create(email!);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Email address cannot be null or empty");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@domain.com")]
    [InlineData("user@")]
    [InlineData("user.domain.com")]
    public void InvoiceEmailAddress_ShouldReturnFailure_WhenEmailFormatIsInvalid(string email)
    {
        // Act
        var result = InvoiceEmailAddress.Create(email);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Invalid email format");
    }
}
