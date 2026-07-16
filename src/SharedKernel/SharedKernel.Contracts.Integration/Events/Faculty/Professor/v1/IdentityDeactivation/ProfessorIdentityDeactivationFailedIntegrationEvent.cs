namespace SharedKernel.Contracts.Integration.Events.Faculty.Professor.v1.IdentityDeactivation;

public sealed record ProfessorIdentityDeactivationFailedIntegrationEvent : IntegrationEvent
{
    public Guid ProfessorId { get; init; }
    public Guid IdentityUserId { get; init; }
    public string Reason { get; init; } = null!;
}