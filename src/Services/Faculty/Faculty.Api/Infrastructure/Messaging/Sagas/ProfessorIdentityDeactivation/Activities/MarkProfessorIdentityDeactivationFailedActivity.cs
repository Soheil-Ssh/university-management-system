using Faculty.Api.Features.Professors.v1.IdentityDeactivation;
using Faculty.Api.Infrastructure.Messaging.Sagas.ProfessorIdentityDeactivation.States;
using SharedKernel.Contracts.Integration.Events.Faculty.Professor.v1.IdentityDeactivation;

namespace Faculty.Api.Infrastructure.Messaging.Sagas.ProfessorIdentityDeactivation.Activities;

public class MarkProfessorIdentityDeactivationFailedActivity(ISender sender) : 
    IStateMachineActivity<ProfessorIdentityDeactivationState, ProfessorIdentityDeactivationFailedIntegrationEvent>
{
    public async Task Execute(BehaviorContext<ProfessorIdentityDeactivationState, ProfessorIdentityDeactivationFailedIntegrationEvent> context, IBehavior<ProfessorIdentityDeactivationState, ProfessorIdentityDeactivationFailedIntegrationEvent> next)
    {
        var result = await sender.Send(new MarkProfessorIdentityDeactivationFailed
            .Command(context.Message.ProfessorId, context.Message.Reason));

        if (result.IsFailure)
            throw new InvalidOperationException(
                $"Failed to mark professor identity deactivate as failed. " +
                $"EmployeeId: {context.Message.ProfessorId}, " +
                $"Reason: {context.Message.Reason}, " +
                $"Error: {result.Error.ToString()}");

        await next.Execute(context);
    }

    public Task Faulted<TException>(
        BehaviorExceptionContext<ProfessorIdentityDeactivationState, ProfessorIdentityDeactivationFailedIntegrationEvent, TException> context, 
        IBehavior<ProfessorIdentityDeactivationState, ProfessorIdentityDeactivationFailedIntegrationEvent> next) where TException : Exception
    {
        return next.Faulted(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope(nameof(MarkProfessorIdentityDeactivationFailedActivity));
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }
}