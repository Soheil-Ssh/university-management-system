namespace SharedKernel.Contracts.Integration.Events.Faculty.Professor.v1.IdentityDeactivation;

public sealed record ProfessorIdentityDeactivationSucceededIntegrationEvent : IntegrationEvent
{
    public Guid ProfessorId { get; init; }
    public Guid IdentityUserId { get; init; }
}