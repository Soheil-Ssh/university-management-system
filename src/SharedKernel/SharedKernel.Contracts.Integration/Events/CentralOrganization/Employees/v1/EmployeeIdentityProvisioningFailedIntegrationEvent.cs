namespace SharedKernel.Contracts.Integration.Events.CentralOrganization.Employees.v1;

public sealed record EmployeeIdentityProvisioningFailedIntegrationEvent : IntegrationEvent
{
    public Guid EmployeeId { get; init; }
    public string Reason { get; init; } = string.Empty;
}