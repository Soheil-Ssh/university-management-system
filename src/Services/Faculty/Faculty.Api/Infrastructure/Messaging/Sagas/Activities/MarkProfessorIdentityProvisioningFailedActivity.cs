using Faculty.Api.Features.Professors.v1.IdentityProvisioning;
using Faculty.Api.Infrastructure.Messaging.Sagas.States;
using SharedKernel.Contracts.Integration.Events.Faculty.Professor.v1.IdentityProvisioning;

namespace Faculty.Api.Infrastructure.Messaging.Sagas.Activities;

public class MarkProfessorIdentityProvisioningFailedActivity(ISender sender, ILogger<MarkProfessorIdentityProvisioningFailedActivity> logger)
    : IStateMachineActivity<ProfessorIdentityProvisioningState, ProfessorIdentityProvisioningFailedIntegrationEvent>
{
    public async Task Execute(BehaviorContext<ProfessorIdentityProvisioningState, ProfessorIdentityProvisioningFailedIntegrationEvent> context, IBehavior<ProfessorIdentityProvisioningState, ProfessorIdentityProvisioningFailedIntegrationEvent> next)
    {
        var result = await sender.Send(new MarkProfessorIdentityProvisioningFailed
            .Command(context.Message.ProfessorId, context.Message.Reason), context.CancellationToken);

        if (result.IsFailure)
            throw new InvalidOperationException(
                $"Failed to mark professor identity provisioning as failed. " +
                $"EmployeeId: {context.Message.ProfessorId}, " +
                $"Reason: {context.Message.Reason}, " +
                $"Error: {result.Error}");
    }

    public Task Faulted<TException>(
        BehaviorExceptionContext<ProfessorIdentityProvisioningState, ProfessorIdentityProvisioningFailedIntegrationEvent, TException> context, IBehavior<ProfessorIdentityProvisioningState, ProfessorIdentityProvisioningFailedIntegrationEvent> next)
        where TException : Exception
    {
        return next.Faulted(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope(nameof(MarkProfessorIdentityProvisioningFailedActivity));
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }
}