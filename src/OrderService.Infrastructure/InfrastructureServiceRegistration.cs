// © 2025 Behrouz Rad. All rights reserved.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Persistence;
using OrderService.Application.Services;
using OrderService.Domain.Repositories;
using OrderService.Infrastructure.Data;
using OrderService.Infrastructure.Interceptors;
using OrderService.Infrastructure.Repositories;
using OrderService.Infrastructure.Services;

namespace OrderService.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrderDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                builder => builder.MigrationsAssembly("OrderService.Api"))
            .AddInterceptors(new CreatedAtInterceptor()));

        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<OrderDbContext>());
        services.AddScoped<IDatabaseMigrationService, DatabaseMigrationService>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IStockValidationService, StockValidationService>();

        return services;
    }
}
