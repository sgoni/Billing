namespace Billing.Application.Dtos;

public record InvoiceDto(
    Guid Id,
    string Number,
    DateTime IssueDate,
    Guid? CustomerId,
    List<InvoiceLineDto> Lines
);