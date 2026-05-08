namespace Billing.Application.Features.Invoices.Commands.UpdateApInvoice;

public record UpdatelInvoiceCommand(InvoiceDto Invoice) : ICommand<UpdatelInvoiceResult>;

public record UpdatelInvoiceResult(bool IsSuccess);

public class UpdatelInvoiceCommandValidator : AbstractValidator<UpdatelInvoiceCommand>
{
    public UpdatelInvoiceCommandValidator()
    {
        RuleFor(x => x.Invoice.CustomerId).NotEmpty().WithMessage("Customer Id is required.");
        RuleFor(x => x.Invoice.Number).NotEmpty().WithMessage("Invoice number is required.");
        RuleForEach(x => x.Invoice.Lines).SetValidator(new InvoiceLineValidator());
        // Ejemplo: The total of the invoice must be greater than zero
        RuleFor(x => x.Invoice.Lines)
            .Must(lines => lines.Sum(l => l.Price) > 0)
            .WithMessage("El total de la factura debe ser mayor a cero.");
    }
}