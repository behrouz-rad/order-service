// © 2025 Behrouz Rad. All rights reserved.

namespace OrderService.Application.Services;

public interface IDatabaseMigrationService
{
    public Task MigrateAsync();
}
