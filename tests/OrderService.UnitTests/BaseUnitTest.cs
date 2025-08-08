// © 2025 Behrouz Rad. All rights reserved.

using System.Reflection;
using Mapster;

namespace OrderService.UnitTests;

public abstract class BaseUnitTest
{
    static BaseUnitTest()
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetAssembly(typeof(OrderService.Application.ApplicationServiceRegistration))!);
    }
}
