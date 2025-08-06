// © 2025 Behrouz Rad. All rights reserved.

using FluentAssertions;
using OrderService.Domain.Orders;

namespace OrderService.UnitTests.Domain.ValueObjects;

public class InvoiceAddressTests
{
    [Theory]
    [InlineData("123 Sample Street, Berlin")]
    [InlineData("456 Main Avenue, 10001 New York")]
    [InlineData("789 Oak Road, Los Angeles CA 90210")]
    public void InvoiceAddress_ShouldCreateValidAddress_WhenValidAddressProvided(string address)
    {
        // Act
        var result = InvoiceAddress.Create(address);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(address.Trim());
    }

    [Fact]
    public void InvoiceAddress_ShouldReturnFailure_WhenAddressIsNull()
    {
        // Act
        var result = InvoiceAddress.Create(null!);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Address cannot be null or empty");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void InvoiceAddress_ShouldReturnFailure_WhenAddressIsNullOrEmpty(string? address)
    {
        // Act
        var result = InvoiceAddress.Create(address!);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Address cannot be null or empty");
    }

    [Theory]
    [InlineData("123")]
    [InlineData("Short")]
    [InlineData("12345")]
    public void InvoiceAddress_ShouldReturnFailure_WhenAddressTooShort(string address)
    {
        // Act
        var result = InvoiceAddress.Create(address);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Address must be at least 10 characters long");
    }
}
