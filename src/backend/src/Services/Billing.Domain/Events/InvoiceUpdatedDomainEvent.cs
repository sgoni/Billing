namespace Billing.Domain.Events;

public record InvoiceUpdatedDomainEvent(Invoice Before, Invoice After) : IDomainEvent;