namespace Billing.Domain.Models;

/// <summary>
///     Change log for compliance
/// </summary>
public class AuditLog : Entity<AuditLogId>
{
    private AuditLog()
    {
    } // EF

    public string Entity { get; private set; } = default!;
    public string Action { get; private set; } = default!; // Consider enum
    public DateTime PerformedAt { get; private set; }
    public string? Details { get; private set; }

    public static AuditLog Create(AuditLogId id, string entity, string action, string? details = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entity);
        ArgumentException.ThrowIfNullOrWhiteSpace(action);

        return new AuditLog
        {
            Id = id,
            Entity = entity,
            Action = action,
            PerformedAt = DateTime.UtcNow,
            Details = details
        };
    }
}