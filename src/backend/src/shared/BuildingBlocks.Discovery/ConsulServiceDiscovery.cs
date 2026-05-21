namespace BuildingBlocks.Discovery;

public class ConsulServiceDiscovery : IServiceDiscovery
{
    private readonly ConsulConfig _config;
    private readonly IConsulClient _consulClient;
    private readonly ILogger<ConsulServiceDiscovery> _logger;

    public ConsulServiceDiscovery(IOptions<ConsulConfig> options, ILogger<ConsulServiceDiscovery> logger)
    {
        _logger = logger;
        _config = options.Value;
        _logger.LogInformation($"Connecting to Consul at: {_config.Host}");
        _consulClient = new ConsulClient(config => { config.Address = new Uri(_config.Host); });
    }

    public async Task RegisterServiceAsync(CancellationToken cancellationToken)
    {
        if (_config.HealthCheckUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase) && _config.TLSSkipVerify)
            _logger.LogInformation("⚠️ TLS verification is disabled. This is recommended only for development.");

        var httpCheck = new AgentServiceCheck
        {
            HTTP = _config.HealthCheckUrl,
            TLSSkipVerify = _config.TLSSkipVerify,
            DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
            Interval = TimeSpan.FromSeconds(30)
        };

        var registration = new AgentServiceRegistration
        {
            ID = _config.ServiceId,
            Name = _config.ServiceName,
            Address = _config.ServiceAddress,
            Port = _config.ServicePort,
            Checks = new[] { httpCheck }
        };

        await _consulClient.Agent.ServiceRegister(registration, cancellationToken);
        Console.WriteLine($"Service {_config.ServiceName} registered in Consul");

        var services = _consulClient.Agent.Services().Result.Response;
        foreach (var service in services)
        {
            var checks = _consulClient.Health
                .Checks(_config.ServiceName)
                .Result;

            foreach (var checkResult in checks.Response)
                Console.WriteLine($"{checkResult.ServiceID} - {checkResult.Status.Status}");
        }

        // Deregistrar el servicio cuando la aplicación se apague
        cancellationToken.Register(() =>
        {
            _consulClient.Agent.ServiceDeregister(_config.ServiceId).Wait();
            Console.WriteLine($"Service {_config.ServiceName} deregistered from Consul");
        });
    }

    public async Task DesregisterServiceAsync(CancellationToken cancellationToken)
    {
        _consulClient.Agent.ServiceDeregister(_config.ServiceId).Wait();
        Console.WriteLine($"Service {_config.ServiceName} deregistered from Consul");
    }
}