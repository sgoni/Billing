namespace Billing.Infrastructure.Data.Extensions;

public class InitialData
{
    public static IEnumerable<Invoice> InvoicesWithLines
    {
        get
        {
            /*
             * Invoice 1
             */

            var invoiceOne = Invoice.Create(
                InvoiceId.Of(new Guid("b45e75af-fe29-4c2e-9653-aad9bf5b42ef")),
                CustomerId.Of(new Guid("e44ed594-272c-4978-a3b5-11fb47e9ca12")),
                DateTime.UtcNow
            );

            invoiceOne.AddItem(
                InvoiceId.Of(invoiceOne.Id.Value),
                "IPhone XR 10",
                1,
                Money.Of(85000, "CRC"),
                1
            );

            invoiceOne.AddItem(
                InvoiceId.Of(invoiceOne.Id.Value),
                "Funda protectora IPhone XR 10",
                1,
                Money.Of(8000, "CRC"),
                2
            );

            invoiceOne.AddItem(
                InvoiceId.Of(invoiceOne.Id.Value),
                "Cubo cargador 10 Watts",
                1,
                Money.Of(4500, "CRC"),
                3
            );

            return new List<Invoice> { invoiceOne };
        }
    }
}