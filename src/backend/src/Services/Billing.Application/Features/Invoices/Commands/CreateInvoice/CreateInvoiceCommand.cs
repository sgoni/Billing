namespace Billing.Application.Features.Invoices.Commands.CreateInvoice;

public record CreateInvoiceCommand(InvoiceDto Invoice) : ICommand<CreateInvoiceCommandResult>;

public record CreateInvoiceCommandResult(Guid Id);

public class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.Invoice.Number)
            .NotEmpty().WithMessage("Invoice number is required")
            .MinimumLength(11).WithMessage("Invoice number must be longer than 10 characters");

        RuleForEach(x => x.Invoice.Lines).SetValidator(new InvoiceLineValidator());
        RuleFor(x => x.Invoice.Lines).Must(lines => lines.Count > 0)
            .WithMessage("The detail must have at least 1 line.");
    }
}