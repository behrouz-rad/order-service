namespace OrderService.Infrastructure.Services;

public interface IDatabaseMigrationService
{
    public Task MigrateAsync();
}
