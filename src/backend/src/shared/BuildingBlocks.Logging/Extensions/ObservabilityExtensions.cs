namespace BuildingBlocks.Logging.Extensions;

public static class ObservabilityExtensions
{
    private const string outputTemplate =
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} (trace={TraceId}){NewLine}{Exception}";

    public static IServiceCollection AddObservability(this IServiceCollection services, IConfiguration configuration)
    {
        var service = configuration["logging:Service"]!; // service.namespace
        var serviceName = configuration["logging:ServiceName"]!; // service.name

        // 1) SERILOG
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithEnvironmentName()
            .WriteTo.Console(outputTemplate: outputTemplate)
            .WriteTo.OpenTelemetry(opts =>
            {
                opts.Endpoint = "http://otel-collector:4318/v1/logs";
                opts.Protocol = OtlpProtocol.HttpProtobuf;
                opts.ResourceAttributes = new Dictionary<string, object>
                {
                    ["service.name"] = serviceName,
                    ["service.namespace"] = service
                };
            })
            .CreateLogger();

        // *** CLAVE: conecta ILogger<T> de ASP.NET Core con Serilog ***
        services.AddSerilog(); // <- sin esto, Loki queda vacío

        // 2) OPENTELEMETRY (trazas + métricas)
        services.AddOpenTelemetry()
            .ConfigureResource(r => r
                .AddService(serviceName, serviceNamespace: service)
                .AddTelemetrySdk())
            .WithTracing(t => t
                .AddSource("MassTransit")
                .SetSampler(new AlwaysOnSampler())
                .AddAspNetCoreInstrumentation(o =>
                {
                    // *** filtra ruido para que se vean POST/PUT en Tempo ***
                    o.Filter = ctx =>
                    {
                        var path = ctx.Request.Path.Value ?? string.Empty;
                        return !path.StartsWith("/metrics", StringComparison.OrdinalIgnoreCase)
                               && !path.StartsWith("/health", StringComparison.OrdinalIgnoreCase)
                               && !path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase);
                    };
                })
                .AddHttpClientInstrumentation()
                .AddEntityFrameworkCoreInstrumentation(o =>
                {
                    o.SetDbStatementForText = false;   // no adjuntar el SQL completo al span
                })
                .AddOtlpExporter()) // usa OTEL_EXPORTER_OTLP_ENDPOINT
            .WithMetrics(m => m
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddProcessInstrumentation()
                .AddOtlpExporter());

        return services;
    }
}