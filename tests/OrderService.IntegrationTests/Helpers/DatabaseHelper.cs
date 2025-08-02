// © 2025 Behrouz Rad. All rights reserved.

using Microsoft.EntityFrameworkCore;
using OrderService.Infrastructure.Data;

namespace OrderService.IntegrationTests.Helpers;

public static class DatabaseHelper
{
    private const string TestConnectionString = "Server=SNNBW-B84CYD3\\SQLEXPRESS;Database=Order_Test;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true;Encrypt=false";

    public static OrderDbContext CreateTestContext()
    {
        var options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseSqlServer(TestConnectionString)
            .Options;

        return new OrderDbContext(options);
    }

    public static async Task ResetDatabaseAsync()
    {
        await using var context = CreateTestContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    public static string TestDatabaseConnectionString => TestConnectionString;
}
