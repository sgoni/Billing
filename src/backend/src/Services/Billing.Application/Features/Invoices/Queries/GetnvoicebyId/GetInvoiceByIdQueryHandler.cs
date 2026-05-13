namespace Billing.Application.Features.Invoices.Queries.GetnvoicebyId;

public class GetInvoiceByIdQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetInvoiceByIdQuery, GetInvoiceByIdQueryResult>
{
    public async Task<GetInvoiceByIdQueryResult> Handle(GetInvoiceByIdQuery query, CancellationToken cancellationToken)
    {
        var invoiceId = InvoiceId.Of(query.InvoiceId);
        var invoice = await dbContext.Invoices
            .Include(iv => iv.Items)
            .AsNoTracking()
            .Where(iv => iv.Id == invoiceId)
            .SingleOrDefaultAsync(cancellationToken);

        if (invoice is null) throw EntityNotFoundException.For<Invoice>(query.InvoiceId);

        return new GetInvoiceByIdQueryResult(invoice.ToApInvoiceDto());
    }
}