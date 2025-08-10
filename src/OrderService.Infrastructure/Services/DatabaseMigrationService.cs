// © 2025 Behrouz Rad. All rights reserved.

using Microsoft.Extensions.Logging;
using OrderService.Application.Persistence;
using OrderService.Application.Services;
using Polly;

namespace OrderService.Infrastructure.Services;

public class DatabaseMigrationService(IUnitOfWork unitOfWork, ILogger<DatabaseMigrationService> logger) : IDatabaseMigrationService
{
    public async Task MigrateAsync()
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(5 * retryAttempt),
                onRetry: (exception, timespan, retryCount, context) =>
                {
                    logger.LogWarning("Database migration attempt {RetryCount} failed. Retrying in {Delay}ms. Error: {Error}",
                        retryCount, timespan.TotalMilliseconds, exception.Message);
                });

        await retryPolicy.ExecuteAsync(() =>
        {
            logger.LogInformation("Starting database migration...");

            unitOfWork.Migrate(TimeSpan.FromSeconds(20));

            logger.LogInformation("Database migration completed successfully");

            return Task.CompletedTask;
        });
    }
}
