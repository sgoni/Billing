namespace BuildingBlocks.Messaging.MassTransit;

public static class Extensions
{
    public static IServiceCollection AddMessageBroker(this IServiceCollection services, IConfiguration configuration,
        Assembly? assembly = null)
    {
        // Implement RabbitMQ MassTransit configuration
        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();

            if (assembly != null)
                config.AddConsumers(assembly);

            config.UsingRabbitMq((context, configurator) =>
            {
                var host = configuration["MessageBroker:Host"];

                configurator.Host(host, "/", h =>
                {
                    h.Username(configuration["MessageBroker:UserName"]);
                    h.Password(configuration["MessageBroker:Password"]);
                });

                configurator.ConfigureEndpoints(context);
            });
        });

        // Optional: Hostened Service for Health
        //services.AddMassTransitHostedService();
        return services;
    }

    private static bool IsValidUri(string uri)
    {
        return Uri.TryCreate(uri, UriKind.Absolute, out var resultado)
               && (resultado.Scheme == Uri.UriSchemeHttp || resultado.Scheme == Uri.UriSchemeHttps);
    }
}