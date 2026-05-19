namespace Billing.Application.Features.Invoices.EventHandlers.Domain;

public class InvoiceCreatedDomainEventHandler(
    IApplicationDbContext dbContext,
    IPublishEndpoint publishEndpoint,
    IEventLogRepository eventLogRepository,
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
        if (await featureManager.IsEnabledAsync("InvoiceFullfilment"))
            await PublishIntegrationEvent(domainEvent.Invoice, cancellationToken);
    }

    private async Task PublishIntegrationEvent(Invoice invoice, CancellationToken cancellationToken)
    {
        var integrationEvent = invoice.ToIntegrationEvent();

        // OutboxMessage
        await eventLogRepository.SaveProcessedAsync(
            integrationEvent.GetType().Name,
            JsonSerializer.Serialize(integrationEvent),
            Guid.NewGuid() // correlative
        );

        await publishEndpoint.Publish(integrationEvent, cancellationToken);
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
        var details = JsonSerializer.Serialize(apInvoiceCreatedDomainEvent);

        return AuditLog.Create(
            AuditLogId.Of(Guid.NewGuid()),
            "Invoice",
            "Created",
            details
        );
    }
}