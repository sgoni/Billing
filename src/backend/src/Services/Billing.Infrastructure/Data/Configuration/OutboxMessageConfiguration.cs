namespace Billing.Infrastructure.Data.Configuration;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => EventLogId.Of(value));

        builder.Property(x => x.OccurredOn)
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Content)
            .IsRequired();

        builder.Property(x => x.ProcessedOn)
            .IsRequired(false);

        builder.Property(x => x.CorrelativeId)
            .IsRequired();

        builder.HasIndex(x => x.CorrelativeId)
            .IsUnique(); // evita reprocesamiento
    }
}