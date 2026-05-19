namespace BuildingBlocks.Messaging.Events.Invoices;

public record InvoiceCreatedIntegrationEvent : IntegrationEvent
{
    public InvoiceCreatedIntegrationEvent(
        Guid id,
        string numberInvoice,
        DateTime issueDate,
        Guid? customerId,
        decimal total,
        Guid correlationId,
        IEnumerable<InvoiceLines> lines)
    {
        Id = id;
        NumberInvoice = numberInvoice;
        IssueDate = issueDate;
        CustomerId = customerId;
        Total = total;
        CorrelationId = correlationId;
        Lines = lines;
    }

    public Guid Id { get; set; }
    public string NumberInvoice { get; set; }
    public DateTime IssueDate { get; set; }
    public Guid? CustomerId { get; set; }
    public decimal Total { get; private set; }
    public Guid? CorrelationId { get; set; }
    public IEnumerable<InvoiceLines> Lines { get; set; }
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