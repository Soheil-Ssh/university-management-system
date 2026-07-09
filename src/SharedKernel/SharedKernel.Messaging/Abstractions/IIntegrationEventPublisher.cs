using SharedKernel.Contracts.Integration.Abstractions;

namespace SharedKernel.Messaging.Abstractions;

public interface IIntegrationEventPublisher
{
    Task PublishAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
        where TIntegrationEvent : IntegrationEvent;
}