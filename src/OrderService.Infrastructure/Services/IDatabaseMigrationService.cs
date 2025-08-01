// © 2025 Behrouz Rad. All rights reserved.

namespace OrderService.Infrastructure.Services;

public interface IDatabaseMigrationService
{
    public Task MigrateAsync();
}
