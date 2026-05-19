namespace Billing.Application.Features.Invoices.Commands.UpdateApInvoice;

public class UpdatelApInvoiceHandler(
    IApplicationDbContext dbContext
)
    : ICommandHandler<UpdatelInvoiceCommand, UpdatelInvoiceResult>
{
    public async Task<UpdatelInvoiceResult> Handle(UpdatelInvoiceCommand command, CancellationToken cancellationToken)
    {
        //create invoice entity from command object
        //save to database
        //return result 

        var invoiceId = InvoiceId.Of(command.Invoice.Id);
        var apInvoice = await dbContext.Invoices
            .Include(iv => iv.Items)
            .Where(iv => iv.Id == invoiceId)
            .SingleOrDefaultAsync(cancellationToken);

        if (apInvoice is null) throw EntityNotFoundException.For<Invoice>(invoiceId);

        var vendorInvoiceUpdated = UpdateInvoice(command, apInvoice);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new UpdatelInvoiceResult(true);
    }

    private Invoice UpdateInvoice(UpdatelInvoiceCommand command, Invoice invoice)
    {
        var numberLine = 1;

        var existingLines = invoice.Items.ToDictionary(x => x.Id.Value);
        var incomingLines = command.Invoice.Lines;

        invoice.Update(
            CustomerId.FromNullable(command.Invoice.CustomerId),
            command.Invoice.IssueDate
        );

        foreach (var line in incomingLines)
            if (line.Id != Guid.Empty && existingLines.ContainsKey(line.Id))
            {
                // UPDATE
                invoice.UpdateLine(
                    invoice.Id.Value,
                    LineId.Of(line.Id),
                    line.Description,
                    line.Quantity,
                    Money.Of(line.Price, "CRC")
                );

                existingLines.Remove(line.Id);
            }
            else
            {
                // CREATE
                invoice.AddItem(
                    invoice.Id,
                    line.Description,
                    line.Quantity,
                    Money.Of(line.Price, "CRC"),
                    numberLine++
                );
            }

        // Remove extra lines
        foreach (var lineToDelete in existingLines.Values) invoice.RemoveLine(lineToDelete.Id);

        return invoice;
    }
}