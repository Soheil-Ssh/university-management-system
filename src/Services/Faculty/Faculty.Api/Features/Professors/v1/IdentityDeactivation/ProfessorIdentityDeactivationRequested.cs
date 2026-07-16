using SharedKernel.Contracts.Integration.Events.Faculty.Professor.v1.IdentityDeactivation;

namespace Faculty.Api.Features.Professors.v1.IdentityDeactivation;

public static class ProfessorIdentityDeactivationRequested
{
    public sealed class DomainEventHandler(IIntegrationEventPublisher publisher) : IEventHandler<ProfessorDeactivatedDomainEvent>
    {
        public async Task Handle(ProfessorDeactivatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new ProfessorIdentityDeactivationRequestedIntegrationEvent
            {
                CorrelationId = notification.EventId,
                ProfessorId = notification.ProfessorId.Value,
                IdentityUserId = notification.IdentityUserId.Value
            };

            await publisher.PublishAsync(integrationEvent, cancellationToken);
        }
    }
}