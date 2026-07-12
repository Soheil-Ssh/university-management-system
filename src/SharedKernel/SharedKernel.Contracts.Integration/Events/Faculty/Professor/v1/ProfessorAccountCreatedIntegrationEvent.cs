namespace SharedKernel.Contracts.Integration.Events.Faculty.Professor.v1;

public sealed record ProfessorAccountCreatedIntegrationEvent : IntegrationEvent
{
    public Guid ProfessorId { get; init; }
    public Guid IdentityUserId { get; init; }
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string MobileNumber { get; init; } = null!;
    public string UserName { get; init; } = null!;
    public string TemporaryPassword { get; init; } = null!;
    public bool MustChangePassword { get; init; }
}