namespace Faculty.Api.Features.Professors.v1.IdentityProvisioning;

public static class MarkProfessorIdentityProvisioningFailed
{
    public sealed record Command(Guid ProfessorId, string Reason) : ICommand<Result>;

    public sealed class Handler(IProfessorRepository professorRepository, IUnitOfWork unitOfWork, ILogger<Handler> logger)
        : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var employee = await professorRepository.GetByIdAsync(new ProfessorId(request.ProfessorId), cancellationToken);

            if (employee is null)
            {
                logger.LogWarning("Professor identity provisioning failed event received, but employee was not found. ProfessorId: {ProfessorId}, Reason: {Reason}",
                    request.ProfessorId,
                    request.Reason);

                return ProfessorErrors.NotFound;
            }

            var markResult = employee.MarkIdentityProvisioningFailed(request.Reason);
            if (markResult.IsFailure)
            {
                logger.LogWarning("Failed to mark professor identity provisioning as failed. ProfessorId: {ProfessorId}, Reason: {Reason}, Error: {Error}",
                    request.ProfessorId,
                    request.Reason,
                    markResult.Error.Description);

                return markResult;
            }

            await unitOfWork.SaveAsync(cancellationToken);

            logger.LogInformation("Professor identity provisioning marked as failed. ProfessorId: {ProfessorId}, Reason: {Reason}",
                request.ProfessorId,
                request.Reason);
            return Result.Success();
        }
    }
}