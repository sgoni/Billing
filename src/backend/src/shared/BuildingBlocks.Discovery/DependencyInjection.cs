namespace BuildingBlocks.Discovery;

public static class DependencyInjection
{
    public static IServiceCollection AddDiscovery(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        // Cargar la configuración de Consul desde appsettings.json
        services.Configure<ConsulConfig>(configuration.GetSection("ConsulConfig"));

        // Registrar la clase ConsulServiceRegistrar como un servicio
        services.AddSingleton<IServiceDiscovery, ConsulServiceDiscovery>();

        // Registrar ConsulRegistrationService como un servicio alojado (hosted service)
        services.AddHostedService<ConsulRegistrationService>();

        return services;
    }
}