using MassTransit;
using Microsoft.Extensions.Logging;
using SharedKernel.Contracts.Integration.Abstractions;
using SharedKernel.Messaging.Abstractions;

namespace SharedKernel.Messaging.MassTransit.Consumers;

public sealed class MassTransitIntegrationEventConsumer<TIntegrationEvent>(IEnumerable<IIntegrationEventHandler<TIntegrationEvent>> handlers,
    ILogger<MassTransitIntegrationEventConsumer<TIntegrationEvent>> logger)
    : IConsumer<TIntegrationEvent>
    where TIntegrationEvent : IntegrationEvent
{
    public async Task Consume(ConsumeContext<TIntegrationEvent> context)
    {
        var integrationEvent = context.Message;

        var eventHandlers = handlers.ToArray();

        if (eventHandlers.Length == 0)
        {
            logger.LogWarning("No integration event handler registered for event {EventType}", typeof(TIntegrationEvent).Name);
            return;
        }

        foreach (var handler in eventHandlers)
        {
            await handler.HandleAsync(integrationEvent, context.CancellationToken);
        }
    }
}