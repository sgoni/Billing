namespace Billing.Application.Dtos;

public record InvoiceLineDto(
    Guid Id,
    Guid InvoiceId,
    string Description,
    int Quantity,
    decimal Price,
    int LineNumber,
    decimal Total);