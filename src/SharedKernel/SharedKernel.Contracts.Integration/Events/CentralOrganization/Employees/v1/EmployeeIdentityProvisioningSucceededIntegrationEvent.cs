namespace SharedKernel.Contracts.Integration.Events.CentralOrganization.Employees.v1;

public sealed record EmployeeIdentityProvisioningSucceededIntegrationEvent : IntegrationEvent
{
    public Guid EmployeeId { get; init; }
    public Guid IdentityUserId { get; init; }
}