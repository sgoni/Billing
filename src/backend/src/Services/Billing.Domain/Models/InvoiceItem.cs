namespace Billing.Domain.Models;

public class InvoiceItem : Entity<LineId>
{
    private InvoiceItem()
    {
    } // Necesario para EF

    internal InvoiceItem(InvoiceId invoiceId, string description, int quantity, Money price, int lineNumber)
    {
        Id = LineId.Of(Guid.NewGuid());
        InvoiceId = invoiceId;
        Description = description;
        Quantity = quantity;
        Price = price;
        LineNumber = lineNumber;
    }

    public InvoiceId InvoiceId { get; private set; }
    public string Description { get; private set; }
    public int Quantity { get; private set; }
    public Money Price { get; private set; }
    public int LineNumber { get; private set; }
    public decimal Total => Quantity * Price.Amount;
    public Invoice Invoice { get; private set; } = default!;

    public static InvoiceItem Create(
        InvoiceId invoiceId,
        string description,
        int quantity,
        Money price,
        int lineNumber)
    {
        return new InvoiceItem(invoiceId, description, quantity, price, lineNumber);
    }

    public void Update(Guid invoiceId, string description, int quantity, Money price)
    {
        ArgumentNullException.ThrowIfNull(price);

        InvoiceId = InvoiceId.Of(invoiceId);
        Description = description;
        Quantity = quantity;
        Price = price;
    }
}