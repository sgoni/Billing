namespace Billing.Application.Features.Invoices.Commands.CreateInvoice;

public record CreateInvoiceCommand(InvoiceDto Invoice) : ICommand<CreateInvoiceCommandResult>;

public record CreateInvoiceCommandResult(Guid Id);

public class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.Invoice.Number).NotEmpty().MaximumLength(12).WithMessage("Invoice number is required");
        RuleFor(x => x.Invoice.Lines).Must(lines => lines.Count > 0)
            .WithMessage("The detail must have at least 1 line.");
    }
}