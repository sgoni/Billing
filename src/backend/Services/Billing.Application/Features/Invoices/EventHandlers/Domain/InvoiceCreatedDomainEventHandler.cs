namespace Billing.Application.Features.Invoices.EventHandlers.Domain;

public class InvoiceCreatedDomainEventHandler(
    IApplicationDbContext dbContext,
    IFeatureManager featureManager,
    ILogger<InvoiceCreatedDomainEventHandler> logger)
    : INotificationHandler<InvoiceCreatedDomainEvent>
{
    public async Task Handle(InvoiceCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        // Log the handling of the domain event for observability and diagnostics
        logger.LogInformation("Invoice created: {InvoiceId}", domainEvent.Invoice.Id.Value);

        // Create and persist a new audit log entry for the invoice creation
        await PersistAuditLog(domainEvent, cancellationToken);

        // Publica evento de integración (si aplica)
        //if (await featureManager.IsEnabledAsync("InvoiceFullfilment"))
        //await PublishIntegrationEvent(invoice, cancellationToken);
    }

    private async Task PersistAuditLog(InvoiceCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        // Convert the domain invoice entity to a DTO for audit logging
        var dto = domainEvent.Invoice.ToApInvoiceDto();
        var auditLog = CreateNewAuditLog(dto);

        dbContext.AuditLogs.Add(auditLog);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private AuditLog CreateNewAuditLog(InvoiceDto apInvoiceCreatedDomainEvent)
    {
        var details = JsonSerializer.Serialize(new
        {
            apInvoiceCreatedDomainEvent.Id,
            apInvoiceCreatedDomainEvent.Number,
            apInvoiceCreatedDomainEvent.IssueDate,
            apInvoiceCreatedDomainEvent.CustomerId,
            Lines = apInvoiceCreatedDomainEvent.Lines.Select(l => new
            {
                l.Id,
                l.Price,
                l.Quantity,
                l.Total,
                l.Description,
                l.LineNumber
            })
        });

        return AuditLog.Create(
            AuditLogId.Of(Guid.NewGuid()),
            "Invoice",
            "Created",
            details
        );
    }
}