namespace Billing.Infrastructure.Data.Configuration;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => InvoiceId.Of(value));

        builder.Property(x => x.Number)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.Number)
            .IsUnique();

        builder.Property(x => x.IssueDate)
            .IsRequired();

        builder.Property(x => x.Total)
            .HasPrecision(18, 2);

        // CustomerId (Value Object nullable)
        builder.Property(x => x.CustomerId)
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                value => value != null ? CustomerId.Of(value.Value) : null);

        // Relación con Items
        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey("InvoiceId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}