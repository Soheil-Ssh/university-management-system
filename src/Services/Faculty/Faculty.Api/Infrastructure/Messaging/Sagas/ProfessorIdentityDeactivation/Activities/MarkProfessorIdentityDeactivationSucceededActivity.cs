using Faculty.Api.Features.Professors.v1.IdentityDeactivation;
using Faculty.Api.Infrastructure.Messaging.Sagas.ProfessorIdentityDeactivation.States;
using SharedKernel.Contracts.Integration.Events.Faculty.Professor.v1.IdentityDeactivation;

namespace Faculty.Api.Infrastructure.Messaging.Sagas.ProfessorIdentityDeactivation.Activities;

public class MarkProfessorIdentityDeactivationSucceededActivity(ISender sender) : 
    IStateMachineActivity<ProfessorIdentityDeactivationState, ProfessorIdentityDeactivationSucceededIntegrationEvent>
{
    public async Task Execute(BehaviorContext<ProfessorIdentityDeactivationState, ProfessorIdentityDeactivationSucceededIntegrationEvent> context,
        IBehavior<ProfessorIdentityDeactivationState, ProfessorIdentityDeactivationSucceededIntegrationEvent> next)
    {
        var result = await sender.Send(new MarkProfessorIdentityDeactivationSucceeded
            .Command(context.Message.ProfessorId), context.CancellationToken);

        if (result.IsFailure)
            throw new InvalidOperationException(
                $"Failed to mark professor identity deactivate as succeeded. " +
                $"ProfessorId: {context.Message.ProfessorId}, " +
                $"Error: {result.Error.ToString()}");

        await next.Execute(context);
    }

    public Task Faulted<TException>(
        BehaviorExceptionContext<ProfessorIdentityDeactivationState, ProfessorIdentityDeactivationSucceededIntegrationEvent, TException> context,
        IBehavior<ProfessorIdentityDeactivationState, ProfessorIdentityDeactivationSucceededIntegrationEvent> next) where TException : Exception
    {
        return next.Faulted(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope(nameof(MarkProfessorIdentityDeactivationSucceededActivity));
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }
}