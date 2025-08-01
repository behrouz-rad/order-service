// © 2025 Behrouz Rad. All rights reserved.

using OrderService.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(builder);

builder.Services.AddApiServices()
                .AddApiDocumentation()
                .AddApplicationLayers(builder.Configuration)
                .AddPipelineBehaviors()
                .AddGlobalExceptionHandling();

var app = builder.Build()
                 .ConfigurePipeline();

await app.MigrateDatabaseAsync();

await app.RunAsync();

public partial class Program { }
