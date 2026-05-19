namespace Billing.Application.Abstractions.Persintence;

public interface IApplicationDbContext
{
    DbSet<Invoice> Invoices { get; }
    DbSet<InvoiceItem> InvoiceItems { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<OutboxMessage> EventLogs { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}