namespace Billing.Application.Data;

public class EventLogRepository : IEventLogRepository
{
    private readonly IApplicationDbContext _dbContext;

    public EventLogRepository(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> AlreadyProcessedAsync(Guid messageId)
    {
        return await _dbContext.EventLogs.AnyAsync(e => e.CorrelativeId == messageId);
    }

    public async Task SaveProcessedAsync(string type, string content, Guid correlativeId)
    {
        try
        {
            var eventLog = OutboxMessage.Create(type, content, correlativeId);
            _dbContext.EventLogs.Add(eventLog);
            await _dbContext.SaveChangesAsync(default);
        }
        catch (Exception e)
        {
            throw e.InnerException;
        }
    }
}