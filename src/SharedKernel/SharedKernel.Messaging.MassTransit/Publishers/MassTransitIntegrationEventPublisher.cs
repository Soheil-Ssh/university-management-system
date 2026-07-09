using MassTransit;
using SharedKernel.Contracts.Integration.Abstractions;
using SharedKernel.Messaging.Abstractions;

namespace SharedKernel.Messaging.MassTransit.Publishers;

public sealed class MassTransitIntegrationEventPublisher(IPublishEndpoint publishEndpoint) : IIntegrationEventPublisher
{
    public Task PublishAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
        where TIntegrationEvent : IntegrationEvent
    {
        return publishEndpoint.Publish(integrationEvent, cancellationToken);
    }
}