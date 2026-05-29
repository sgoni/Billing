namespace Billing.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        services.AddVault(configuration); // Add Vault  
        services.AddDiscovery(configuration); // Add Discovery
        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        services.AddScoped<VaultDbConnectionInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
        });

        var environment = configuration["ASPNETCORE_ENVIRONMENT"];

        if (environment == "Development")
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseNpgsql(connectionString);
            });
        
        if (environment == "Production")
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.AddInterceptors(sp.GetServices<VaultDbConnectionInterceptor>());
                options.UseNpgsql();
            });

        return services;
    }
}