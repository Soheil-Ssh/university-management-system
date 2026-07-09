using CentralOrganization.Api.Domain.Employee.Errors;

namespace CentralOrganization.Api.Features.Employees.v1.IdentityProvisioning;

public static class MarkEmployeeIdentityProvisioningFailed
{
    public sealed record Command(Guid EmployeeId, string Reason) : ICommand<Result>;

    public sealed class Handler(IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork, ILogger<Handler> logger)
        : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var employeeId = new EmployeeId(request.EmployeeId);

            var employee = await employeeRepository.GetByIdAsync(
                employeeId,
                cancellationToken);

            if (employee is null)
            {
                logger.LogWarning("Employee identity provisioning failed event received, but employee was not found. EmployeeId: {EmployeeId}, Reason: {Reason}",
                    request.EmployeeId,
                    request.Reason);

                return EmployeeErrors.NotFound;
            }

            var markResult = employee.MarkIdentityProvisioningFailed(request.Reason);

            if (markResult.IsFailure)
            {
                logger.LogWarning("Failed to mark employee identity provisioning as failed. EmployeeId: {EmployeeId}, Reason: {Reason}, Error: {Error}",
                    request.EmployeeId,
                    request.Reason,
                    markResult.Error.Description);

                return markResult;
            }

            await unitOfWork.SaveAsync(cancellationToken);

            logger.LogInformation("Employee identity provisioning marked as failed. EmployeeId: {EmployeeId}, Reason: {Reason}",
                request.EmployeeId,
                request.Reason);
            return Result.Success();
        }
    }
}