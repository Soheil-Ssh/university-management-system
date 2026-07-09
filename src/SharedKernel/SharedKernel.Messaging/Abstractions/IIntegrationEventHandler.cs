using SharedKernel.Contracts.Integration.Abstractions;

namespace SharedKernel.Messaging.Abstractions;

public interface IIntegrationEventHandler<in TIntegrationEvent>
    where TIntegrationEvent : IntegrationEvent
{
    Task HandleAsync(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
}