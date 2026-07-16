namespace SharedKernel.Contracts.Integration.Events.Identity.User.v1;

public record DeactivateProfessorIdentityUserRequestedIntegrationEvent : IntegrationEvent
{
    public Guid ProfessorId { get; init; }
    public Guid IdentityUserId { get; init; }
}