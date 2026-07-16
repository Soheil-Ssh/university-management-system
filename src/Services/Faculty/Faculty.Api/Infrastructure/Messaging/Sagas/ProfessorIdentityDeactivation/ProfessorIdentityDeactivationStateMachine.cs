using Faculty.Api.Infrastructure.Messaging.Sagas.ProfessorIdentityDeactivation.Activities;
using Faculty.Api.Infrastructure.Messaging.Sagas.ProfessorIdentityDeactivation.States;
using SharedKernel.Contracts.Integration.Events.Faculty.Professor.v1.IdentityDeactivation;
using SharedKernel.Contracts.Integration.Events.Identity.User.v1;

namespace Faculty.Api.Infrastructure.Messaging.Sagas.ProfessorIdentityDeactivation;

public class ProfessorIdentityDeactivationStateMachine : MassTransitStateMachine<ProfessorIdentityDeactivationState>
{
    public State WaitingForIdentityDeactivation { get; private set; } = null!;

    public Event<ProfessorIdentityDeactivationRequestedIntegrationEvent> DeactivationRequested { get; private set; } = null!;
    public Event<ProfessorIdentityDeactivationSucceededIntegrationEvent> DeactivationSucceeded { get; private set; } = null!;
    public Event<ProfessorIdentityDeactivationFailedIntegrationEvent> DeactivationFailed { get; private set; } = null!;

    public ProfessorIdentityDeactivationStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => DeactivationRequested, x => x.CorrelateById(context => context.Message.CorrelationId));

        Event(() => DeactivationSucceeded, x => x.CorrelateById(context => context.Message.CorrelationId));

        Event(() => DeactivationFailed, x => x.CorrelateById(context => context.Message.CorrelationId));

        Initially(
            When(DeactivationRequested)
                .Then(context =>
                {
                    context.Saga.ProfessorId = context.Message.ProfessorId;
                    context.Saga.IdentityUserId = context.Message.IdentityUserId;
                    context.Saga.CreatedAt = DateTime.UtcNow;
                })
                .Publish(context => new DeactivateProfessorIdentityUserRequestedIntegrationEvent
                {
                    CorrelationId = context.Saga.CorrelationId,
                    ProfessorId = context.Saga.ProfessorId,
                    IdentityUserId = context.Saga.IdentityUserId
                })
                .TransitionTo(WaitingForIdentityDeactivation));

        During(WaitingForIdentityDeactivation,
            When(DeactivationSucceeded)
                .Activity(x => x.OfType<MarkProfessorIdentityDeactivationSucceededActivity>())
                .Then(context =>
                {
                    context.Saga.CompletedAt = DateTime.UtcNow;
                    context.Saga.FailureReason = null;
                })
                .Finalize(),

            When(DeactivationFailed)
                .Activity(x => x.OfType<MarkProfessorIdentityDeactivationFailedActivity>())
                .Then(context =>
                {
                    context.Saga.CompletedAt = DateTime.UtcNow;
                    context.Saga.FailureReason = context.Message.Reason;
                })
                .Finalize());

        SetCompletedWhenFinalized();
    }
}