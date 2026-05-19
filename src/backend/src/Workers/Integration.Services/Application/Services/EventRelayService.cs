namespace Integration.Services.Application.Services;

public interface IEventRelayService
{
    Task<EventRelayLog?> FindRelayEventAsync(Guid id, CancellationToken cancellationToken);
    Task RelayEventAsync(EventRelayLog log, CancellationToken cancellationToken);
}

/// <summary>
///     Forwarding and persistence logic
/// </summary>
public class EventRelayService : IEventRelayService
{
    private readonly ILogger<EventRelayService> _logger;
    private readonly IDocumentSession _session;

    public EventRelayService(IDocumentSession session, ILogger<EventRelayService> logger)
    {
        _session = session;
        _logger = logger;
    }

    public async Task<EventRelayLog?> FindRelayEventAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _session.LoadAsync<EventRelayLog>(id, cancellationToken);
    }

    public async Task RelayEventAsync(EventRelayLog log, CancellationToken cancellationToken)
    {
        // Log to EventRelayLog
        _session.Store(log);
        await _session.SaveChangesAsync(cancellationToken);
    }
}