namespace Billing.Application.Validators;

public class InvoiceLineValidator : AbstractValidator<InvoiceLineDto>
{
    public InvoiceLineValidator()
    {
        RuleFor(x => x.LineNumber)
            .NotEmpty();

        // Ejemplo: validate that the total number of lines is greater than zero
        RuleFor(x => x)
            .Must(line => line.Price > 0)
            .WithMessage("El precio de la línea debe ser mayor a cero.");
        
        RuleFor(x => x)
            .Must(line => line.Quantity > 0)
            .WithMessage("Cantidad del Item debe de ser al menos 1.");        
    }
}