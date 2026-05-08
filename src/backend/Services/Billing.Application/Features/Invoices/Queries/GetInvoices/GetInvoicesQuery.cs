namespace Billing.Application.Features.Invoices.Queries.GetInvoices;

public record GetInvoicesQuery(PaginationRequest PaginationRequest) : IQuery<GetInvoicesQueryResult>;

public record GetInvoicesQueryResult(PaginatedResult<InvoiceDto> Invoices);