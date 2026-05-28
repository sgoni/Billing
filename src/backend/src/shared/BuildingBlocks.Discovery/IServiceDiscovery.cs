namespace BuildingBlocks.Discovery;

public interface IServiceDiscovery
{
    Task RegisterServiceAsync(CancellationToken cancellationToken);

    Task DesregisterServiceAsync(CancellationToken cancellationToken);
    
    Task<(string Host, int Port)> GetServiceAsync(string serviceName);
}