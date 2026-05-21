namespace BuildingBlocks.Secrets;

public static class DependencyInjection
{
    public static IServiceCollection AddVault(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        // Cargar la configuración de Vault desde appsettings.json
        services.Configure<VaultSettings>(configuration.GetSection("VaultSettings"));

        // Registrar la clase VaultConfigurationProvider como un servicio
        services.AddScoped<ISecretManager, VaultConfigurationProvider>();

        return services;
    }
}