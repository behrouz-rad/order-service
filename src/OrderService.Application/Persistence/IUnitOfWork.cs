// © 2025 Behrouz Rad. All rights reserved.

namespace OrderService.Application.Persistence;

public interface IUnitOfWork : IDisposable
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    public void Migrate(TimeSpan timeout);
}
