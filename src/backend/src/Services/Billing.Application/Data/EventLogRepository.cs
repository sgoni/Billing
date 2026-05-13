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
        return await _dbContext.EventLogs.AnyAsync(e => e.MessageId == messageId);
    }

    public async Task SaveProcessedAsync(Guid messageId)
    {
        var eventLog = EventLog.Create(messageId);
        _dbContext.EventLogs.Add(eventLog);
        await _dbContext.SaveChangesAsync(default);
    }
}