namespace Integration.Services.Infrastructure.Persistence.Configurations;

public class EventRelayLogConfiguration : IEntityTypeConfiguration<EventRelayLog>
{
    public void Configure(EntityTypeBuilder<EventRelayLog> builder)
    {
        builder.ToTable("EventRelayLog");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.EventName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.EventType)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.SourceService)
            .HasMaxLength(250);

        builder.Property(e => e.DestinationService)
            .HasMaxLength(250);

        builder.Property(e => e.Payload)
            .HasMaxLength(1000);

        builder.Property(e => e.CorrelationId)
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(e => e.ProcessedAt);

        builder.Property(e => e.CreatedAt);

        builder.Property(e => e.PublishedAt);

        builder.Property(e => e.ErrorMessage)
            .HasMaxLength(1000);

        builder.HasIndex(e => e.Status);
    }
}