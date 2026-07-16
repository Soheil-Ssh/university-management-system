namespace SharedKernel.Contracts.Integration.Events.Faculty.Professor.v1.IdentityDeactivation;

public record ProfessorIdentityDeactivationRequestedIntegrationEvent : IntegrationEvent
{
    public Guid ProfessorId { get; init; }
    public Guid IdentityUserId { get; init; }
}