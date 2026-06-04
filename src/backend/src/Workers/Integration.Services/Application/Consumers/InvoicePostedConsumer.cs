namespace Integration.Services.Application.Consumers;

public class InvoicePostedConsumer(
    IEventRelayService relayService,
    IPublishEndpoint publishEndpoint,
    ILogger<InvoiceCreatedIntegrationEvent> logger)
    : IConsumer<InvoiceCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<InvoiceCreatedIntegrationEvent> context)
    {
        var evt = context.Message;
        logger.LogInformation("Received InvoiceCreatedIntegrationEvent for InvoiceId: {InvoiceId}", evt.Id);

        // Idempotencia
        var eventRelayLog = relayService.FindRelayEventAsync(evt.CorrelationId, new CancellationToken());
        if (eventRelayLog is null)
        {
            logger.LogWarning("Duplicate InvoiceCreatedIntegrationEvent detected ({CorrelationId})",
                evt.CorrelationId);
            return;
        }

        var relayLog = CreateNewEventLogRelay(
            nameof(InvoiceCreatedIntegrationEvent),
            "CxP",
            "Invoice",
            "InvoicePosted",
            JsonSerializer.Serialize(evt),
            evt.CorrelationId!,
            EventRelayStatus.Published);

        // Log to EventRelayLog
        await relayService.RelayEventAsync(relayLog, new CancellationToken());

        // Resend to AP.API
        var invoicePostedIntegrationEvent = evt.Adapt<InvoiceCreatedIntegrationEvent>();

        // To..Do
        // Relaunch the event to another consumer
        //await publishEndpoint.Publish(invoicePostedIntegrationEvent);
        logger.LogInformation("Relayed InvoicePostedIntegrationEvent to Accounting.API");
    }

    private EventRelayLog CreateNewEventLogRelay(string eventName, string eventType, string sourceService,
        string destinationService, string payload, Guid? evtCorrelationId, EventRelayStatus status)
    {
        var relayLog = EventRelayLog.CreateNew(eventName, eventType, sourceService, destinationService, payload,
            evtCorrelationId, status);

        return relayLog;
    }
}