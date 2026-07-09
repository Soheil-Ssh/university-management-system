using CentralOrganization.Api.Infrastructure.Messaging.Sagas.Activities;
using CentralOrganization.Api.Infrastructure.Messaging.Sagas.States;
using SharedKernel.Contracts.Integration.Events.CentralOrganization.Employees.v1;
using SharedKernel.Contracts.Integration.Events.Identity.User.v1;

namespace CentralOrganization.Api.Infrastructure.Messaging.Sagas;

public class EmployeeIdentityProvisioningStateMachine : MassTransitStateMachine<EmployeeIdentityProvisioningState>
{
    public State WaitingForIdentityUserCreation { get; set; } = null!;

    public Event<EmployeeIdentityProvisioningRequestedIntegrationEvent> ProvisioningRequested { get; set; } = null!;
    public Event<EmployeeIdentityProvisioningSucceededIntegrationEvent> ProvisioningSucceeded { get; set; } = null!;
    public Event<EmployeeIdentityProvisioningFailedIntegrationEvent> ProvisioningFailed { get; set; } = null!;

    public EmployeeIdentityProvisioningStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => ProvisioningRequested, r =>
        {
            r.CorrelateById(context => context.Message.EmployeeId);
            r.SelectId(context => context.Message.EmployeeId);
        });

        Event(() => ProvisioningSucceeded, s =>
        {
            s.CorrelateById(context => context.Message.EmployeeId);
        });

        Event(() => ProvisioningFailed, f =>
        {
            f.CorrelateById(context => context.Message.EmployeeId);
        });

        Initially(
            When(ProvisioningRequested)
                .Then(context =>
                {
                    context.Saga.EmployeeId = context.Message.EmployeeId;
                    context.Saga.NationalCode = context.Message.NationalCode;
                    context.Saga.FirstName = context.Message.FirstName;
                    context.Saga.LastName = context.Message.LastName;
                    context.Saga.Email = context.Message.Email;
                    context.Saga.MobileNumber = context.Message.MobileNumber;
                    context.Saga.CreatedAt = DateTime.UtcNow;
                })
                .Send(
                    new Uri("queue:identity-create-employee-user"),
                    context => new CreateEmployeeIdentityUserRequestedIntegrationEvent()
                    {
                        EmployeeId = context.Message.EmployeeId,
                        NationalCode = context.Message.NationalCode,
                        FirstName = context.Message.FirstName,
                        LastName = context.Message.LastName,
                        Email = context.Message.Email,
                        MobileNumber = context.Message.MobileNumber,
                        CorrelationId = context.Message.EmployeeId
                    })
                .TransitionTo(WaitingForIdentityUserCreation));

        During(WaitingForIdentityUserCreation,
            When(ProvisioningSucceeded)
                .Then(context =>
                {
                    context.Saga.IdentityUserId = context.Message.IdentityUserId;
                    context.Saga.CompletedAt = DateTime.UtcNow;
                })
                .Activity(x => x.OfType<MarkEmployeeIdentityProvisioningSucceededActivity>())
                .Finalize(),

            When(ProvisioningFailed)
                .Then(context =>
                {
                    context.Saga.FailureReason = context.Message.Reason;
                    context.Saga.CompletedAt = DateTime.UtcNow;
                })
                .Activity(x => x.OfType<MarkEmployeeIdentityProvisioningFailedActivity>())
                .Finalize());

        SetCompletedWhenFinalized();
    }
}