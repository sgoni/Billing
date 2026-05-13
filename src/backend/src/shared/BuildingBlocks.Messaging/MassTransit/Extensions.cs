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
                var hostUri = configuration["MessageBroker:Host"]!;
                var userName = configuration["MessageBroker:UserName"];
                var password = configuration["MessageBroker:Password"];
                Console.WriteLine($"Connecting to RabbitMQ at: {hostUri}");

                if (string.IsNullOrWhiteSpace(hostUri))
                    throw new ArgumentNullException(nameof(hostUri), "MessageBroker:Host is not configured");

                configurator.Host(new Uri(hostUri!), host =>
                {
                    if (!string.IsNullOrWhiteSpace(userName)) host.Username(userName);
                    if (!string.IsNullOrWhiteSpace(password)) host.Password(password);
                });

                if (IsValidUri(hostUri))
                    configurator.Host(hostUri, host =>
                    {
                        if (!string.IsNullOrWhiteSpace(userName))
                            host.Username(userName);
                        if (!string.IsNullOrWhiteSpace(password))
                            host.Password(password);
                    });
                else
                    configurator.Host(hostUri, host =>
                    {
                        if (!string.IsNullOrWhiteSpace(userName))
                            host.Username(userName);
                        if (!string.IsNullOrWhiteSpace(password))
                            host.Password(password);
                    });

                // Enable to schedule deferred messages (delayed reset)
                //configurator.UseMessageRetry(r =>
                //{
                //    // Exponential: 5 attempts, base 1s, max 30s, factor 2
                //    r.Exponential(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(2));
                //    // Ignore Reintento for Business Errors
                //    //r.Ignore<BusinessRuleValidationException>();
                //    //r.Ignore<ValidationException>();
                //});
//
                //// Deferred Redelivery (if the handler explicitly asks for Delay)
                //configurator.UseScheduledRedelivery(rd =>
                //{
                //    rd.Intervals(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(2));
                //});

                // Error / Dead-Letter that are by default:
                // - <Queue> _error for messages that definitely fail
                // - <Queue> _Skipped for discarded messages by topology

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