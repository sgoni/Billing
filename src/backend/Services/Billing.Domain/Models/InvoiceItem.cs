namespace Billing.Domain.Models;

public class InvoiceItem : Entity<LineId>
{
    private InvoiceItem()
    {
    } // Necesario para EF

    public InvoiceItem(string description, int quantity, Money price, int lineNumber)
    {
        Description = description;
        Quantity = quantity;
        Price = price;
        LineNumber = lineNumber;
    }

    public string Description { get; private set; }
    public int Quantity { get; private set; }
    public Money Price { get; private set; }
    public int LineNumber { get; private set; }
    public decimal Total => Quantity * Price.Amount;

    public static InvoiceItem Create(string description, int quantity, Money price, int lineNumber)
    {
        return new InvoiceItem(description, quantity, price, lineNumber);
    }

    public void Update(string description, int quantity, Money price)
    {
        ArgumentNullException.ThrowIfNull(price);

        Description = description;
        Quantity = quantity;
        Price = price;
    }
}