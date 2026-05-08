namespace Billing.Application.Features.Invoices.Queries.GetInvoices;

public class GetInvoicesQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetInvoicesQuery, GetInvoicesQueryResult>
{
    public async Task<GetInvoicesQueryResult> Handle(GetInvoicesQuery query, CancellationToken cancellationToken)
    {
        // get apinvoices with pagination
        // return result

        var pageIndex = query.PaginationRequest.PageIndex;
        var pageSize = query.PaginationRequest.PageSize;
        var totalCount = await dbContext.InvoiceItems.LongCountAsync(cancellationToken);

        var apInvoices = await dbContext.Invoices
            .Include(vi => vi.Items)
            .AsNoTracking()
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new GetInvoicesQueryResult(
            new PaginatedResult<InvoiceDto>(pageIndex,
                pageSize,
                totalCount,
                apInvoices.ToInvoiceDtoList()));
    }
}