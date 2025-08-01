// © 2025 Behrouz Rad. All rights reserved.

using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Common;
using OrderService.Domain.Entities;
using OrderService.Domain.Repositories;

namespace OrderService.Infrastructure.Repositories;

public class OrderRepository(IUnitOfWork unitOfWork) : IOrderRepository
{
    private readonly DbSet<Order> _orders = unitOfWork.DbSet<Order>();

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _orders.AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        return await _orders.AsNoTracking()
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber, cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _orders.AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        _orders.Add(order);
        return Task.FromResult(order);
    }

    public Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        _orders.Update(order);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Order order, CancellationToken cancellationToken = default)
    {
        _orders.Remove(order);
        return Task.CompletedTask;
    }
}
