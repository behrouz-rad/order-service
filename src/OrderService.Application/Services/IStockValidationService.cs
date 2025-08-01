// © 2025 Behrouz Rad. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.Services;
public interface IStockValidationService
{
    public Task<bool> IsProductInStockAsync(string productId, int requestedAmount, CancellationToken cancellationToken = default);
}
