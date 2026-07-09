namespace SharedKernel.Contracts.Integration.Events.CentralOrganization.Employees.v1;

public record EmployeeIdentityProvisioningRequestedIntegrationEvent : IntegrationEvent
{
    public Guid EmployeeId { get; init; }
    public Guid UnitId { get; init; }
    public string PersonnelCode { get; init; } = string.Empty;
    public string NationalCode { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string MobileNumber { get; init; } = string.Empty;
}