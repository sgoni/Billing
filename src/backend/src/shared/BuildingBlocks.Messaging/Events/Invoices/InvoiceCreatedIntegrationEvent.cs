namespace BuildingBlocks.Messaging.Events.Invoices;

public record InvoiceCreatedIntegrationEvent : IntegrationEvent
{
    //public InvoiceCreatedIntegrationEvent(
    //    Guid id,
    //    string numberInvoice,
    //    DateTime issueDate,
    //    Guid? customerId,
    //    decimal total,
    //    Guid correlationId,
    //    IEnumerable<InvoiceLines> lines)
    //{
    //    Id = id;
    //    NumberInvoice = numberInvoice;
    //    IssueDate = issueDate;
    //    CustomerId = customerId;
    //    Total = total;
    //    CorrelationId = correlationId;
    //    Lines = lines;
    //}

    public Guid Id { get; init; }
    public string NumberInvoice { get; init; } = default!;
    public DateTime IssueDate { get; init; }
    public Guid? CustomerId { get; init; }
    public decimal Total { get; init; }
    public Guid? CorrelationId { get; init; }
    public IEnumerable<InvoiceLines> Lines { get; init; } = [];
}

public record InvoiceLines
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public int LineNumber { get; set; }
    public decimal Total { get; set; }
}