namespace Billing.Domain.Models;

public class EventLog : Entity<EventLogId>
{
    private EventLog()
    {
    } // EF

    private EventLog(EventLogId id, Guid messageId, DateTime processedAt)
    {
        if (messageId == Guid.Empty)
            throw new DomainException("MessageId cannot be empty");

        Id = id;
        MessageId = messageId;
        ProcessedAt = processedAt;
    }

    public Guid MessageId { get; private set; }
    public DateTime ProcessedAt { get; private set; }

    public static EventLog Create(Guid messageId)
    {
        return new EventLog(
            EventLogId.Of(Guid.NewGuid()),
            messageId,
            DateTime.UtcNow
        );
    }
}