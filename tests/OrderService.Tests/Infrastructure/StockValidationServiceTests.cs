// © 2025 Behrouz Rad. All rights reserved.

using FluentAssertions;
using OrderService.Infrastructure.Services;

namespace OrderService.Tests.Infrastructure;

public class StockValidationServiceTests
{
    private readonly StockValidationService _stockValidationService;

    public StockValidationServiceTests()
    {
        _stockValidationService = new StockValidationService();
    }

    public sealed class StockValidationTestCase
    {
        public string TestName { get; init; } = string.Empty;
        public string ProductId { get; init; } = string.Empty;
        public int RequestedAmount { get; init; }
        public bool ExpectedResult { get; init; }
        public string Description { get; init; } = string.Empty;

        public override string ToString() => TestName;
    }

    public static IEnumerable<object[]> StockValidationTestCases()
    {
        var testCases = new List<StockValidationTestCase>
        {
            new()
            {
                TestName = "GamingLaptop_SufficientStock_ReturnsTrue",
                ProductId = "12345",
                RequestedAmount = 5,
                ExpectedResult = true,
                Description = "Gaming Laptop with 10 in stock, requesting 5"
            },
            new()
            {
                TestName = "GamingLaptop_ExactStock_ReturnsTrue",
                ProductId = "12345",
                RequestedAmount = 10,
                ExpectedResult = true,
                Description = "Gaming Laptop with 10 in stock, requesting exact amount (10)"
            },
            new()
            {
                TestName = "GamingLaptop_InsufficientStock_ReturnsFalse",
                ProductId = "12345",
                RequestedAmount = 11,
                ExpectedResult = false,
                Description = "Gaming Laptop with 10 in stock, requesting more than available (11)"
            },
            new()
            {
                TestName = "NonExistentProduct_ReturnseFalse",
                ProductId = "INVALID",
                RequestedAmount = 1,
                ExpectedResult = false,
                Description = "Non-existent product ID, requesting 1"
            },
            new()
            {
                TestName = "EmptyProductId_ReturnsFalse",
                ProductId = "",
                RequestedAmount = 1,
                ExpectedResult = false,
                Description = "Empty product ID, requesting 1"
            },
            new()
            {
                TestName = "NullProductId_ReturnsFalse",
                ProductId = null!,
                RequestedAmount = 1,
                ExpectedResult = false,
                Description = "Null product ID, requesting 1"
            }
        };

        return testCases.Select(tc => new object[] { tc });
    }

    [Theory]
    [MemberData(nameof(StockValidationTestCases))]
    public async Task IsProductInStockAsync_VariousScenarios_ReturnsExpectedResult(StockValidationTestCase testCase)
    {
        // Act
        var result = await _stockValidationService.IsProductInStockAsync(testCase.ProductId, testCase.RequestedAmount);

        // Assert
        result.Should().Be(testCase.ExpectedResult, testCase.Description);
    }
}
