namespace Billing.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration,
        Assembly assembly)
    {
        services.AddCarter();

        services.AddExceptionHandler<CustomExceptionHandler>();

        // Add Health Checks
        services
            .AddHealthChecks()
            .AddApplicationStatus("api_status", tags: new[] { "api" })
            .AddNpgSql(configuration.GetConnectionString("Database")!,
                name: "sql",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "db", "sql", "Npgsql" });

        // Add Controllers
        services.AddControllers();

        // Add Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddHttpContextAccessor();

        //To..Do
        //services.AddDiscovery(configuration);         // Add Discovery
        //services.AddObservability(configuration);     // Add Telemetry
        return services;
    }

    public static WebApplication UseApiServices(this WebApplication app)
    {
        app.MapCarter();

        // Add Middleware
        app.UseHttpsRedirection();
        //app.UseAuthorization();

        // Map Controllers
        app.MapControllers();

        // Use Exception Handler
        app.UseExceptionHandler(options => { });

        // Add Health Checks
        app.UseHealthChecks("/health",
            new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

        //app.UseOpenTelemetryPrometheusScrapingEndpoint(); // Map the /metrics endpoint
        
        return app;
    }
}