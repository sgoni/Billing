namespace Billing.Domain.Events;

public record InvoiceCreatedDomainEvent(Invoice Invoice) : IDomainEvent;