namespace IntegrationService.Application.Consumers;

public class InvoiceCreatedConsumer(
    IEventRelayService relayService,
    IPublishEndpoint publishEndpoint,
    ILogger<InvoiceCreatedConsumer> logger) : IConsumer<InvoiceCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<InvoiceCreatedIntegrationEvent> context)
    {
        var evt = context.Message;
        logger.LogInformation("Received InvoiceCreatedIntegrationEvent for Invoice Id: {Id}", evt.Id);

        // Idempotencia
        if (evt.CorrelationId != null)
        {
            var eventRelayLog = relayService.FindRelayEventAsync(evt.CorrelationId.Value, new CancellationToken());

            if (eventRelayLog is null)
            {
                logger.LogWarning("Duplicate PeriodClosedIntegrationEvent detected ({CorrelationId})",
                    evt.CorrelationId);
                return;
            }
        }

        var relayLog = CreateNewEventLogRelay(
            nameof(InvoiceCreatedIntegrationEvent),
            "Integration",
            "Accounting.API",
            "AP.API",
            JsonSerializer.Serialize(evt),
            evt.CorrelationId!,
            EventRelayStatus.Published);

        // Log to EventRelayLog
        await relayService.RelayEventAsync(relayLog, new CancellationToken());

        // Resend to AP.API
        var integrationEvent = evt.Adapt<InvoiceCreatedIntegrationEvent>();
        await publishEndpoint.Publish(integrationEvent);
        logger.LogInformation("Relayed integrationEvent to ConsumerI");
    }

    private EventRelayLog CreateNewEventLogRelay(string eventName, string eventType, string sourceService,
        string destinationService, string payload, Guid? correlationId, EventRelayStatus status)
    {
        var relayLog = EventRelayLog.CreateNew(eventName, eventType, sourceService, destinationService, payload,
            correlationId, status);

        return relayLog;
    }
}