namespace SharedKernel.Contracts.Integration.Events.CentralOrganization.Employees.v1;

public sealed record EmployeeAccountCreatedIntegrationEvent : IntegrationEvent
{
    public Guid EmployeeId { get; init; }
    public Guid IdentityUserId { get; init; }
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string MobileNumber { get; init; } = null!;
    public string UserName { get; init; } = null!;
    public string TemporaryPassword { get; init; } = null!;
    public bool MustChangePassword { get; init; }
}