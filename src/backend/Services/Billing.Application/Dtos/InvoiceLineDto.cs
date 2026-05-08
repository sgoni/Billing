namespace Billing.Application.Dtos;

public record InvoiceLineDto(
    Guid Id,
    string Description,
    int Quantity,
    decimal Price,
    int LineNumber,
    decimal Total);