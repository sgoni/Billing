namespace Billing.Application.Extensions;

public static class InvoiceExtensions
{
    public static IEnumerable<InvoiceDto> ToInvoiceDtoList(this IEnumerable<Invoice> Invoices)
    {
        return Invoices.Select(invoice => new InvoiceDto(
            invoice.Id.Value,
            invoice.Number,
            invoice.IssueDate,
            invoice.CustomerId.Value,
            invoice.Total,
            invoice.Items.Select(i => new InvoiceLineDto(
                i.Id.Value,
                i.InvoiceId.Value,
                i.Description,
                i.Quantity,
                i.Price.Amount,
                i.LineNumber,
                i.Total)).ToList()
        ));
    }

    public static InvoiceDto ToApInvoiceDto(this Invoice invoice)
    {
        return DtoFromApinvoice(invoice);
    }

    private static InvoiceDto DtoFromApinvoice(Invoice invoice)
    {
        return new InvoiceDto(
            invoice.Id.Value,
            invoice.Number,
            invoice.IssueDate,
            invoice.CustomerId.Value,
            invoice.Total,
            invoice.Items.Select(i => new InvoiceLineDto(
                i.Id.Value,
                i.InvoiceId.Value,
                i.Description,
                i.Quantity,
                i.Price.Amount,
                i.LineNumber,
                i.Total)).ToList()
        );
    }

    public static InvoiceCreatedIntegrationEvent ToIntegrationEvent(this Invoice invoice)
    {
        return new InvoiceCreatedIntegrationEvent(
            invoice.Id.Value,
            invoice.Number,
            invoice.IssueDate,
            invoice.CustomerId?.Value,
            invoice.Total,
            Guid.NewGuid(), // o el que venga del contexto
            invoice.Items.Select(x => new InvoiceLines
            {
                Id = x.Id.Value,
                Description = x.Description,
                Quantity = x.Quantity,
                Price = x.Price.Amount,
                LineNumber = x.LineNumber,
                Total = x.Total
            })
        );
    }
}