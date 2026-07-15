namespace SharedKernel.Contracts.Integration.Events.Faculty.Professor.v1.IdentityProvisioning;

public record ProfessorIdentityProvisioningSucceededIntegrationEvent : IntegrationEvent
{
    public Guid ProfessorId { get; init; }
    public Guid IdentityUserId { get; init; }
}