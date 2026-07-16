namespace Faculty.Api.Infrastructure.Messaging.Sagas.ProfessorIdentityDeactivation.States;

public sealed class ProfessorIdentityDeactivationState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = null!;
    public Guid ProfessorId { get; set; }
    public Guid IdentityUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? FailureReason { get; set; }
}