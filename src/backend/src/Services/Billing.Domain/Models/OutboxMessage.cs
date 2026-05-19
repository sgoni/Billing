namespace Billing.Domain.Models;

public class OutboxMessage : Entity<EventLogId>
{
    private OutboxMessage()
    {
    } // EF

    private OutboxMessage(EventLogId id, string type, string content, Guid correlativeId)
    {
        if (correlativeId == Guid.Empty)
            throw new DomainException("MessageId cannot be empty");

        Id = id;
        OccurredOn = DateTime.UtcNow;
        Type = type;
        Content = content;
        CorrelativeId = correlativeId;
        ProcessedOn = DateTime.UtcNow;
    }

    public DateTime OccurredOn { get; set; }
    public string Type { get; set; } = default!;
    public string Content { get; set; } = default!;
    public DateTime? ProcessedOn { get; set; }
    public Guid CorrelativeId { get; set; }

    public static OutboxMessage Create(string type, string content, Guid correlativeId)
    {
        return new OutboxMessage(
            EventLogId.Of(Guid.NewGuid()),
            type,
            content,
            correlativeId
        );
    }
}