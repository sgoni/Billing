public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        // PK
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasConversion(
                id => id.Value,
                value => InvoiceId.Of(value))
            .ValueGeneratedNever();

        // Properties
        builder.Property(i => i.Number)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.IssueDate)
            .IsRequired();

        builder.Property(i => i.Total)
            .HasPrecision(18, 2);

        // CustomerId (nullable VO)
        builder.Property(i => i.CustomerId)
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                value => value != null ? CustomerId.Of(value.Value) : null);

        // Relationship (IMPORTANTE)
        builder.HasMany(x => x.Items)
            .WithOne(x => x.Invoice)
            .HasForeignKey(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignorar DomainEvents si aplica
        builder.Ignore("DomainEvents");
    }
}