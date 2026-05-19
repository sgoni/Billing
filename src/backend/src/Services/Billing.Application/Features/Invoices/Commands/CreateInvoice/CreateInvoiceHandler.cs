namespace Billing.Application.Features.Invoices.Commands.CreateInvoice;

public class CreateInvoiceHandler(IApplicationDbContext dbContext)
    : ICommandHandler<CreateInvoiceCommand, CreateInvoiceCommandResult>
{
    public async Task<CreateInvoiceCommandResult> Handle(CreateInvoiceCommand command,
        CancellationToken cancellationToken)
    {
        //create invoice entity from command object
        //save to database
        //return result 

        var newInvoice = CreatenewInvoice(command.Invoice);

        dbContext.Invoices.Add(newInvoice);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateInvoiceCommandResult(newInvoice.Id.Value);
    }

    private Invoice CreatenewInvoice(InvoiceDto command)
    {
        var invoiceId = InvoiceId.Of(Guid.NewGuid());

        //Create header
        var invoice = Invoice.Create(
            invoiceId,
            CustomerId.FromNullable(command.CustomerId),
            DateTime.UtcNow
        );

        //Add details
        var lineNumber = 0;
        foreach (var item in command.Lines)
            invoice.AddItem(
                invoiceId,
                item.Description,
                item.Quantity,
                Money.Of(item.Price, "CRC"),
                lineNumber++);

        return invoice;
    }
}