// © 2025 Behrouz Rad. All rights reserved.

using OrderService.Application.Services;

namespace OrderService.Infrastructure.Services;
public class StockValidationService : IStockValidationService
{
    // Simple hardcoded stock validation for demonstration
    // In a real scenario, this would query a database or external service
    private static readonly Dictionary<string, int> ProductStock = new()
    {
        { "12345", 10 },     // Gaming Laptop - in stock
        { "67890", 0 },      // Mouse - out of stock  
        { "54321", 5 },      // Keyboard - in stock
        { "99999", 0 },      // More out of stock product for testing
    };

    public Task<bool> IsProductInStockAsync(string productId, int requestedAmount, CancellationToken cancellationToken = default)
    {
        if (!ProductStock.TryGetValue(productId, out int availableStock))
        {
            return Task.FromResult(false);
        }

        var isInStock = availableStock >= requestedAmount;

        return Task.FromResult(isInStock);
    }
}
