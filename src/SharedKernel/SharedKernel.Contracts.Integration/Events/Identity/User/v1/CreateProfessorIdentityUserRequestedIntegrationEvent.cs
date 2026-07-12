namespace SharedKernel.Contracts.Integration.Events.Identity.User.v1;

public sealed record CreateProfessorIdentityUserRequestedIntegrationEvent : IntegrationEvent
{
    public Guid ProfessorId { get; init; }
    public string NationalCode { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string MobileNumber { get; init; } = string.Empty;
}