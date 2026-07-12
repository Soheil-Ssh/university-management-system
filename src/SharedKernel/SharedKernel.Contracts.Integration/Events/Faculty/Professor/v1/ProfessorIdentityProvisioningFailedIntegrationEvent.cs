namespace SharedKernel.Contracts.Integration.Events.Faculty.Professor.v1;

public record ProfessorIdentityProvisioningFailedIntegrationEvent : IntegrationEvent
{
    public Guid ProfessorId { get; init; }
    public string Reason { get; init; } = string.Empty;
}