namespace Billing.Application.Data;

public interface IEventLogRepository
{
    Task<bool> AlreadyProcessedAsync(Guid messageId);
    Task SaveProcessedAsync(Guid messageId);
}