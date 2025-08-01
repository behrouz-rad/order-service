// © 2025 Behrouz Rad. All rights reserved.

namespace OrderService.Application.Services;
public interface IStockValidationService
{
    public Task<bool> IsProductInStockAsync(string productId, int requestedAmount, CancellationToken cancellationToken = default);
}
