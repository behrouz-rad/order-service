// © 2025 Behrouz Rad. All rights reserved.

using OrderService.Domain.Orders;

namespace OrderService.Domain.Repositories;

public interface IOrderRepository
{
    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    public Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    public Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default);
    public Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
    public Task DeleteAsync(Order order, CancellationToken cancellationToken = default);
}
