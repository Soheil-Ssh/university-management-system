using SharedKernel.Contracts.Integration.Events.Faculty.Professor.v1.IdentityProvisioning;

namespace Faculty.Api.Features.Professors.v1.IdentityProvisioning;

public static class ProfessorIdentityProvisioningRequested
{
    public sealed class DomainEventHandler(IIntegrationEventPublisher integrationEventPublisher) : IEventHandler<ProfessorIdentityProvisioningRequestedDomainEvent>
    {
        public Task Handle(ProfessorIdentityProvisioningRequestedDomainEvent notification, CancellationToken cancellationToken)
        {
            var integrationEvent = new ProfessorIdentityProvisioningRequestedIntegrationEvent(
                notification.ProfessorId,
                notification.NationalCode,
                notification.FirstName,
                notification.LastName,
                notification.Email,
                notification.MobileNumber
            );

            return integrationEventPublisher.PublishAsync(integrationEvent, cancellationToken);
        }
    }
}