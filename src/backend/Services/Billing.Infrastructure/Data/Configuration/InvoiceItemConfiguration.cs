namespace Billing.Infrastructure.Data.Configuration;

public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.ToTable("InvoiceItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => LineId.Of(value));

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.LineNumber)
            .IsRequired();

        // Money como Value Object (Owned)
        builder.OwnsOne(x => x.Price, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("PriceAmount")
                .HasPrecision(18, 2);

            money.Property(m => m.CurrencyCode)
                .HasColumnName("PriceCurrency")
                .HasMaxLength(10);
        });

        // Ignorar propiedad calculada
        builder.Ignore(x => x.Total);
    }
}