using CentralOrganization.Api.Features.Employees.v1.IdentityProvisioning;
using CentralOrganization.Api.Infrastructure.Messaging.Sagas.States;
using SharedKernel.Contracts.Integration.Events.CentralOrganization.Employees.v1;

namespace CentralOrganization.Api.Infrastructure.Messaging.Sagas.Activities;

public sealed class MarkEmployeeIdentityProvisioningFailedActivity(ISender sender)
    : IStateMachineActivity<EmployeeIdentityProvisioningState, EmployeeIdentityProvisioningFailedIntegrationEvent>
{
    public async Task Execute(
        BehaviorContext<EmployeeIdentityProvisioningState, EmployeeIdentityProvisioningFailedIntegrationEvent> context,
        IBehavior<EmployeeIdentityProvisioningState, EmployeeIdentityProvisioningFailedIntegrationEvent> next)
    {
        await sender.Send(
            new MarkEmployeeIdentityProvisioningFailed.Command(
                context.Message.EmployeeId,
                context.Message.Reason),
            context.CancellationToken);

        await next.Execute(context);
    }

    public Task Faulted<TException>(
        BehaviorExceptionContext<EmployeeIdentityProvisioningState, EmployeeIdentityProvisioningFailedIntegrationEvent, TException> context,
        IBehavior<EmployeeIdentityProvisioningState, EmployeeIdentityProvisioningFailedIntegrationEvent> next)
        where TException : Exception
    {
        return next.Faulted(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope(nameof(MarkEmployeeIdentityProvisioningFailedActivity));
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }
}