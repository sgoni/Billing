public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.ToTable("InvoiceItems");

        // PK
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasConversion(
                id => id.Value,
                value => LineId.Of(value))
            .ValueGeneratedNever();

        // FK (Value Object)
        builder.Property(i => i.InvoiceId)
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => InvoiceId.Of(value));

        // Properties
        builder.Property(i => i.Description)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(i => i.Quantity)
            .IsRequired();

        builder.Property(i => i.LineNumber)
            .IsRequired();

        // Money (Owned Entity)
        builder.OwnsOne(i => i.Price, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("PriceAmount")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(m => m.CurrencyCode)
                .HasColumnName("PriceCurrency")
                .HasMaxLength(10)
                .IsRequired();
        });
    }
}