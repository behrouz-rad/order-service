using Microsoft.Extensions.Logging;
using OrderService.Domain.Common;
using Polly;
using static System.Console;

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
            WriteLine("Started MigrateDb");

            unitOfWork.Migrate(TimeSpan.FromMinutes(7));

            logger.LogInformation("Database migration completed successfully");
            WriteLine("Finished MigrateDb");

            return Task.CompletedTask;
        });
    }
}
