namespace Billing.Application.Data;

public interface IEventLogRepository
{
    Task<bool> AlreadyProcessedAsync(Guid messageId);
    Task SaveProcessedAsync(string type, string content, Guid correlativeId);
}