// © 2025 Behrouz Rad. All rights reserved.

using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace OrderService.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(assembly));

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
