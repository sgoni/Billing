namespace Billing.Infrastructure.Data.Configuration;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => AuditLogId.Of(value));

        builder.Property(x => x.Entity)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Action)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.PerformedAt)
            .IsRequired();

        builder.Property(x => x.Details)
            .HasMaxLength(2000);
    }
}