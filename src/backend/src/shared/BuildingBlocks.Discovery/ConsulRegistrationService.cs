namespace BuildingBlocks.Discovery;

public class ConsulRegistrationService : BackgroundService
{
    private readonly IServiceDiscovery _serviceDiscovery;

    public ConsulRegistrationService(IServiceDiscovery serviceDiscovery)
    {
        _serviceDiscovery = serviceDiscovery;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _serviceDiscovery.RegisterServiceAsync(stoppingToken);
        // Aquí puedes añadir lógica adicional si necesitas hacer ping o renovación periódica en Consul
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        // Puedes agregar aquí cualquier limpieza antes de detener el servicio
        await _serviceDiscovery
            .DesregisterServiceAsync(cancellationToken); // Este método podría desregistrar el servicio
        await base.StopAsync(cancellationToken);
    }
}