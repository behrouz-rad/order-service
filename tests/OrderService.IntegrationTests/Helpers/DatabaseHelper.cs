// © 2025 Behrouz Rad. All rights reserved.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OrderService.Infrastructure.Data;

namespace OrderService.IntegrationTests.Helpers;

public static class DatabaseHelper
{
    private static string GetTestConnectionString()
    {
        var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Test";
        return new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile($"appsettings.{env}.json", optional: false, reloadOnChange: false)
            .Build()
            .GetConnectionString("TestConnection")!;
    }

    public static OrderDbContext CreateTestContext()
    {
        var options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseSqlServer(TestDatabaseConnectionString)
            .Options;
        return new OrderDbContext(options);
    }

    public static async Task ResetDatabaseAsync()
    {
        await using var context = CreateTestContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    public static string TestDatabaseConnectionString => GetTestConnectionString();
}
