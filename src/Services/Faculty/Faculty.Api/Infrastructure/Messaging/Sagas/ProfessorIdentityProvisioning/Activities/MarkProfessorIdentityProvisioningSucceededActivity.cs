using Faculty.Api.Features.Professors.v1.IdentityProvisioning;
using Faculty.Api.Infrastructure.Messaging.Sagas.ProfessorIdentityProvisioning.States;
using SharedKernel.Contracts.Integration.Events.Faculty.Professor.v1.IdentityProvisioning;

namespace Faculty.Api.Infrastructure.Messaging.Sagas.ProfessorIdentityProvisioning.Activities;

public class MarkProfessorIdentityProvisioningSucceededActivity(ISender sender, ILogger<MarkProfessorIdentityProvisioningSucceededActivity> logger) :
    IStateMachineActivity<ProfessorIdentityProvisioningState, ProfessorIdentityProvisioningSucceededIntegrationEvent>
{
    public async Task Execute(BehaviorContext<ProfessorIdentityProvisioningState, ProfessorIdentityProvisioningSucceededIntegrationEvent> context,
        IBehavior<ProfessorIdentityProvisioningState, ProfessorIdentityProvisioningSucceededIntegrationEvent> next)
    {
        var result = await sender.Send(
            new MarkProfessorIdentityProvisioningSucceeded.Command(context.Message.ProfessorId, context.Message.IdentityUserId),
            context.CancellationToken);

        if (result.IsFailure)
            throw new InvalidOperationException(
                $"Failed to mark professor identity provisioning as succeeded. " +
                $"ProfessorId: {context.Message.ProfessorId}, " +
                $"Error: {result.Error}");

        await next.Execute(context);
    }

    public Task Faulted<TException>(BehaviorExceptionContext<ProfessorIdentityProvisioningState, ProfessorIdentityProvisioningSucceededIntegrationEvent, TException> context,
        IBehavior<ProfessorIdentityProvisioningState, ProfessorIdentityProvisioningSucceededIntegrationEvent> next)
        where TException : Exception
    {
        return next.Faulted(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope(nameof(MarkProfessorIdentityProvisioningSucceededActivity));
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }
}