using CentralOrganization.Api.Features.Employees.v1.IdentityProvisioning;
using CentralOrganization.Api.Infrastructure.Messaging.Sagas.States;
using SharedKernel.Contracts.Integration.Events.CentralOrganization.Employees.v1;

namespace CentralOrganization.Api.Infrastructure.Messaging.Sagas.Activities;

public class MarkEmployeeIdentityProvisioningSucceededActivity(ISender sender)
    : IStateMachineActivity<EmployeeIdentityProvisioningState, EmployeeIdentityProvisioningSucceededIntegrationEvent>
{
    public async Task Execute(BehaviorContext<EmployeeIdentityProvisioningState, EmployeeIdentityProvisioningSucceededIntegrationEvent> context,
        IBehavior<EmployeeIdentityProvisioningState, EmployeeIdentityProvisioningSucceededIntegrationEvent> next)
    {
        var result = await sender.Send(
            new MarkEmployeeIdentityProvisioningSucceeded.Command(context.Message.EmployeeId, context.Message.IdentityUserId),
            context.CancellationToken);

        if (result.IsFailure)
            throw new InvalidOperationException(
                $"Failed to mark employee identity provisioning as succeeded. " +
                $"EmployeeId: {context.Message.EmployeeId}, " +
                $"Error: {result.Error}");

        await next.Execute(context);
    }

    public Task Faulted<TException>(BehaviorExceptionContext<EmployeeIdentityProvisioningState,
        EmployeeIdentityProvisioningSucceededIntegrationEvent, TException> context,
        IBehavior<EmployeeIdentityProvisioningState, EmployeeIdentityProvisioningSucceededIntegrationEvent> next) 
        where TException : Exception
    {
        return next.Faulted(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope(nameof(MarkEmployeeIdentityProvisioningSucceededActivity));
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }
}