namespace Faculty.Api.Infrastructure.Messaging.Sagas.States;

public sealed class ProfessorIdentityProvisioningState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public Guid ProfessorId { get; set; }
    public string NationalCode { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string CurrentState { get; set; } = string.Empty;
    public Guid? IdentityUserId { get; set; }
    public string? FailureReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}