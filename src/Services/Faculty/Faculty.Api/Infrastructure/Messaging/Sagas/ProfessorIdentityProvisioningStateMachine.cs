using Faculty.Api.Infrastructure.Messaging.Sagas.Activities;
using Faculty.Api.Infrastructure.Messaging.Sagas.States;
using SharedKernel.Contracts.Integration.Events.CentralOrganization.Employees.v1;
using SharedKernel.Contracts.Integration.Events.Faculty.Professor.v1.IdentityProvisioning;
using SharedKernel.Contracts.Integration.Events.Identity.User.v1;

namespace Faculty.Api.Infrastructure.Messaging.Sagas;

public class ProfessorIdentityProvisioningStateMachine : MassTransitStateMachine<ProfessorIdentityProvisioningState>
{
    public State WaitingForIdentityUserCreation { get; set; } = null!;

    public Event<ProfessorIdentityProvisioningRequestedIntegrationEvent> ProvisioningRequested { get; set; } = null!;
    public Event<ProfessorIdentityProvisioningSucceededIntegrationEvent> ProvisioningSucceeded { get; set; } = null!;
    public Event<ProfessorIdentityProvisioningFailedIntegrationEvent> ProvisioningFailed { get; set; } = null!;

    public ProfessorIdentityProvisioningStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => ProvisioningRequested, r =>
        {
            r.CorrelateById(context => context.Message.ProfessorId);
            r.CorrelateById(context => context.Message.ProfessorId);
        });

        Initially(
            When(ProvisioningRequested)
                .Then(context =>
                {
                    context.Saga.ProfessorId = context.Message.ProfessorId;
                    context.Saga.NationalCode = context.Message.NationalCode;
                    context.Saga.FirstName = context.Message.FirstName;
                    context.Saga.LastName = context.Message.LastName;
                    context.Saga.Email = context.Message.Email;
                    context.Saga.MobileNumber = context.Message.MobileNumber;
                    context.Saga.CreatedAt = DateTime.UtcNow;
                })
                .Send(
                    new Uri("queue:identity-create-professor-user"),
                    context => new CreateProfessorIdentityUserRequestedIntegrationEvent
                    {
                        ProfessorId = context.Message.ProfessorId,
                        NationalCode = context.Message.NationalCode,
                        FirstName = context.Message.FirstName,
                        LastName = context.Message.LastName,
                        Email = context.Message.Email,
                        MobileNumber = context.Message.MobileNumber,
                        CorrelationId = context.Message.ProfessorId
                    })
                .TransitionTo(WaitingForIdentityUserCreation));

        During(WaitingForIdentityUserCreation,
            When(ProvisioningSucceeded)
                .Then(context =>
                {
                    context.Saga.IdentityUserId = context.Message.IdentityUserId;
                    context.Saga.CompletedAt = DateTime.UtcNow;
                })
                .Activity(x => x.OfType<MarkProfessorIdentityProvisioningSucceededActivity>())
                .Publish(context => new ProfessorAccountCreatedIntegrationEvent()
                {
                    ProfessorId = context.Saga.ProfessorId,
                    IdentityUserId = context.Saga.IdentityUserId!.Value,
                    FirstName = context.Saga.FirstName,
                    LastName = context.Saga.LastName,
                    MobileNumber = context.Saga.MobileNumber,
                    UserName = context.Saga.NationalCode,
                    TemporaryPassword = context.Saga.NationalCode,
                    MustChangePassword = true,
                })
                .Finalize(),

            When(ProvisioningFailed)
                .Then(context =>
                {
                    context.Saga.FailureReason = context.Message.Reason;
                    context.Saga.CompletedAt = DateTime.UtcNow;
                })
                .Activity(x => x.OfType<MarkProfessorIdentityProvisioningFailedActivity>())
                .Finalize());

        SetCompletedWhenFinalized();
    }
}