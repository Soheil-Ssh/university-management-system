using CentralOrganization.Api.Domain.Employee.Errors;
using SharedKernel.Domain.Identifiers;

namespace CentralOrganization.Api.Features.Employees.v1.IdentityProvisioning;

public class MarkEmployeeIdentityProvisioningSucceeded
{
    public sealed record Command(Guid EmployeeId, Guid IdentityUserId) : ICommand<Result>;

    public sealed class Handler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork, ILogger<Handler> logger)
        : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var employeeId = new EmployeeId(request.EmployeeId);
            var identityUserId = new UserId(request.IdentityUserId);

            var employee = await employeeRepository.GetByIdAsync(employeeId, cancellationToken);
            if (employee is null)
            {
                logger.LogWarning("Employee identity provisioning succeeded event received, but employee was not found. EmployeeId: {EmployeeId}, IdentityUserId: {IdentityUserId}",
                    request.EmployeeId,
                    request.IdentityUserId);
                return EmployeeErrors.NotFound;
            }

            var markResult = employee.MarkIdentityProvisioningSucceeded(identityUserId);
            if (markResult.IsFailure)
            {
                logger.LogWarning("Failed to mark employee identity provisioning as succeeded. EmployeeId: {EmployeeId}, IdentityUserId: {IdentityUserId}, Error: {Error}",
                    request.EmployeeId,
                    request.IdentityUserId,
                    markResult.Error.Description);
                return markResult;
            }

            await unitOfWork.SaveAsync(cancellationToken);

            logger.LogInformation("Employee identity provisioning marked as succeeded. EmployeeId: {EmployeeId}, IdentityUserId: {IdentityUserId}",
                request.EmployeeId,
                request.IdentityUserId);

            return Result.Success();
        }
    }
}