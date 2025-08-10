// © 2025 Behrouz Rad. All rights reserved.

using OrderService.Application.Services;
using Serilog;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using System.Text;

namespace OrderService.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseExceptionHandler();
        app.UseHttpsRedirection();
        app.UseSerilogRequestLogging();

        app.MapControllers();
        
        app.MapHealthChecks("/health");
        app.MapHealthChecks("/health/detailed", new HealthCheckOptions
        {
            ResponseWriter = WriteHealthCheckResponseAsync
        });

        return app;
    }

    private static async Task WriteHealthCheckResponseAsync(HttpContext context, HealthReport healthReport)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var options = new JsonWriterOptions { Indented = true };
        await using var memoryStream = new MemoryStream();
        await using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString("status", healthReport.Status.ToString());
            jsonWriter.WriteString("totalDuration", healthReport.TotalDuration.ToString());
            jsonWriter.WriteStartObject("results");

            foreach (var (key, value) in healthReport.Entries)
            {
                jsonWriter.WriteStartObject(key);
                jsonWriter.WriteString("status", value.Status.ToString());
                jsonWriter.WriteString("duration", value.Duration.ToString());
                jsonWriter.WriteString("description", value.Description);

                if (value.Data.Count > 0)
                {
                    jsonWriter.WriteStartObject("data");
                    foreach (var (dataKey, dataValue) in value.Data)
                    {
                        jsonWriter.WritePropertyName(dataKey);
                        JsonSerializer.Serialize(jsonWriter, dataValue, dataValue?.GetType() ?? typeof(object));
                    }
                    jsonWriter.WriteEndObject();
                }

                if (!string.IsNullOrEmpty(value.Exception?.Message))
                {
                    jsonWriter.WriteString("exception", value.Exception.Message);
                }

                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();
            jsonWriter.WriteEndObject();
        }

        await context.Response.WriteAsync(Encoding.UTF8.GetString(memoryStream.ToArray()));
    }

    public static async Task<WebApplication> MigrateDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var migrationService = scope.ServiceProvider.GetRequiredService<IDatabaseMigrationService>();

        try
        {
            await migrationService.MigrateAsync();
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<WebApplication>>();
            logger.LogError(ex, "An error occurred while migrating the database");
            throw;
        }

        return app;
    }
}
