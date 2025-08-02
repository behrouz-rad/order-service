// © 2025 Behrouz Rad. All rights reserved.

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrderService.Infrastructure.Data;
using OrderService.IntegrationTests.Helpers;

namespace OrderService.IntegrationTests;
public class OrderWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IDbContextOptionsConfiguration<OrderDbContext>>();

            services.AddDbContext<OrderDbContext>(options =>
            {
                options.UseSqlServer(DatabaseHelper.TestDatabaseConnectionString);
            });
        });
    }

    public async Task InitializeAsync()
    {
        await DatabaseHelper.ResetDatabaseAsync();
    }

    public new async Task DisposeAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

        await context.Database.EnsureDeletedAsync();

        await base.DisposeAsync();
    }
}
