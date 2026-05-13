namespace IntegrationService.BackgroundTasks;

public class EventRelayWorker : BackgroundService
{
    private readonly ILogger<EventRelayWorker> _logger;

    public EventRelayWorker(ILogger<EventRelayWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Axenta.IntegrationService started at: {time}", DateTimeOffset.Now);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}