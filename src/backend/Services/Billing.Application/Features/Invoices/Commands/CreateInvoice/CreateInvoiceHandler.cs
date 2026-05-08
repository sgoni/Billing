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

        throw new NotImplementedException();
    }

    private Invoice CreatenewInvoice(InvoiceDto command)
    {
        //Create header
        var invoice = Invoice.Create(InvoiceId.Of(Guid.NewGuid()),
            CustomerId.FromNullable(command.CustomerId),
            DateTime.UtcNow
        );

        //Add details
        var lineNumber = 1;
        foreach (var item in command.Lines)
        {
            invoice.AddItem(item.Description, item.Quantity, Money.Of(item.Price, "CRC"), lineNumber);
            lineNumber++;
        }

        return invoice;
    }
}