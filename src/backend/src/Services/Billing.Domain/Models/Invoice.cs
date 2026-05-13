namespace Billing.Domain.Models;

public class Invoice : Aggregate<InvoiceId>
{
    private readonly List<InvoiceItem> _items = new();

    private Invoice()
    {
    } // Necesario para EF

    public string Number { get; private set; } = default!;
    public DateTime IssueDate { get; private set; }
    public CustomerId? CustomerId { get; private set; }
    public IReadOnlyCollection<InvoiceItem> Items => _items.AsReadOnly();
    public decimal Total { get; private set; }

    public static Invoice Create(InvoiceId id, CustomerId? customerId, DateTime issueDate)
    {
        ArgumentNullException.ThrowIfNull(customerId);

        var invoice = new Invoice
        {
            Id = id,
            CustomerId = customerId,
            IssueDate = issueDate,
            Number = $"INV-{DateTime.UtcNow.Ticks}"
        };

        // Domain event
        invoice.AddDomainEvent(new InvoiceCreatedDomainEvent(invoice));

        return invoice;
    }

    public void Update(CustomerId customerId, DateTime issueDate)
    {
        var before = new Invoice
        {
            Id = Id,
            CustomerId = customerId,
            IssueDate = issueDate
        };

        CustomerId = customerId;
        IssueDate = issueDate;

        // Domain event
        AddDomainEvent(new InvoiceUpdatedDomainEvent(before, this));
    }

    public void AddItem(string description, int quantity, Money price, int lineNumber = 1)
    {
        var item = InvoiceItem.Create(description, quantity, price, lineNumber);
        _items.Add(item);
        RecalculateTotals();
    }

    public void UpdateLine(LineId idLine, string description, int quantity, Money price)
    {
        var line = _items.SingleOrDefault(x => x.Id == idLine);

        if (line is null)
            throw new DomainException($"Line {line.Id} not found.");

        line.Update(description, quantity, price);

        RecalculateTotals();
    }

    public void RemoveLine(LineId lineId)
    {
        var line = _items.SingleOrDefault(x => x.Id == lineId);

        if (line is null)
            throw new DomainException($"Line {lineId} not found.");

        _items.Remove(line);

        RecalculateTotals();
    }

    private void RecalculateTotals()
    {
        if (!_items.Any())
        {
            Total = 0;
            return;
        }

        Total = _items.Sum(x => x.Total);
    }
}