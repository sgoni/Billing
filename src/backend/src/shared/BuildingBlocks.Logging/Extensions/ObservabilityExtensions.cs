namespace BuildingBlocks.Logging.Extensions;

public static class ObservabilityExtensions
{
    // Define el template de salida una vez (asumiendo que está definido en tu archivo)
    private const string outputTemplate =
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

    public static IServiceCollection AddObservability(this IServiceCollection services, IConfiguration configuration)
    {
        var service = configuration["logging:Service"]!;
        var serviceName = configuration["logging:ServiceName"]!;
        var lokiUrl = configuration["logging:LokiUrl"]!;

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.WithThreadId()
            .WriteTo.Console(outputTemplate: outputTemplate)
            .Enrich.FromLogContext()
            .WriteTo.GrafanaLoki(
                lokiUrl,
                new List<LokiLabel> { new() { Key = service, Value = serviceName } },
                ["app"])
            .CreateLogger();

        // 🚀 OpenTelemetry Resource (identidad del servicio)
        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(serviceName) // Usar el nombre de servicio proporcionado
            .AddTelemetrySdk(); // Buena práctica para traces y metrics

        // El resto de la configuración de OpenTelemetry (Traces y Metrics) sigue igual, 
        // pero apunta al servicio 'alloy:4317' para trazas y métricas si Alloy lo soporta
        services.AddOpenTelemetry()
            .WithTracing(tracerProvider => tracerProvider
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(o =>
                {
                    o.Endpoint = new Uri("http://tempo:4317");
                    o.Protocol = OtlpExportProtocol.Grpc;
                }))
            .WithMetrics(metricsProvider => metricsProvider
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation()
                .AddPrometheusExporter());
        return services;
    }
}