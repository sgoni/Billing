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
            invoice.Items.Select(i => new InvoiceLineDto(
                i.Id.Value,
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
            invoice.Items.Select(i => new InvoiceLineDto(
                i.Id.Value,
                i.Description,
                i.Quantity,
                i.Price.Amount,
                i.LineNumber,
                i.Total)).ToList()
        );
    }
}