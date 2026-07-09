using CentralOrganization.Api.Domain.Employee.Events;
using SharedKernel.Contracts.Integration.Events.CentralOrganization.Employees.v1;

namespace CentralOrganization.Api.Features.Employees.v1.IdentityProvisioning;

public sealed class EmployeeIdentityProvisioningRequestedDomainEventHandler(IIntegrationEventPublisher integrationEventPublisher)
    : IEventHandler<EmployeeIdentityProvisioningRequestedDomainEvent>
{
    public async Task Handle(EmployeeIdentityProvisioningRequestedDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new EmployeeIdentityProvisioningRequestedIntegrationEvent
        {
            EmployeeId = notification.EmployeeId,
            NationalCode = notification.NationalCode,
            FirstName = notification.FirstName,
            LastName = notification.LastName,
            Email = notification.Email,
            MobileNumber = notification.MobileNumber,
            CorrelationId = notification.EmployeeId
        };

        await integrationEventPublisher.PublishAsync(integrationEvent, cancellationToken);
    }
}