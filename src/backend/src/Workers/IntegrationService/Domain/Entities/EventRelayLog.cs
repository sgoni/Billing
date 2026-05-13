namespace IntegrationService.Domain.Entities;

public class EventRelayLog
{
    public Guid Id { get; private set; }
    public string EventName { get; private set; } = default!;
    public string EventType { get; private set; } = default!;
    public string SourceService { get; private set; } = default!;
    public string DestinationService { get; private set; } = default!;
    public string Payload { get; private set; } = default!;
    public Guid? CorrelationId { get; private set; }
    public EventRelayStatus Status { get; private set; } = EventRelayStatus.Pending;
    public DateTime ProcessedAt { get; private set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? PublishedAt { get; private set; }
    public string? ErrorMessage { get; private set; }

    public static EventRelayLog CreateNew(string eventName, string eventType, string sourceService,
        string destinationService, string payload, Guid? correlationId, EventRelayStatus status)
    {
        return new EventRelayLog
        {
            Id = Guid.NewGuid(),
            EventName = eventName,
            EventType = eventType,
            SourceService = sourceService,
            DestinationService = destinationService,
            Payload = payload,
            CorrelationId = correlationId,
            Status = status
        };
    }

    public void MarkAsPublished()
    {
        Status = EventRelayStatus.Published;
        PublishedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string error)
    {
        Status = EventRelayStatus.Failed;
        ErrorMessage = error;
    }
}