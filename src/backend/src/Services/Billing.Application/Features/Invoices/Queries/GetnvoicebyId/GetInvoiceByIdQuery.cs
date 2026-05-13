namespace Billing.Application.Features.Invoices.Queries.GetnvoicebyId;

public record GetInvoiceByIdQuery(Guid InvoiceId) : IQuery<GetInvoiceByIdQueryResult>;

public record GetInvoiceByIdQueryResult(InvoiceDto Invoice);