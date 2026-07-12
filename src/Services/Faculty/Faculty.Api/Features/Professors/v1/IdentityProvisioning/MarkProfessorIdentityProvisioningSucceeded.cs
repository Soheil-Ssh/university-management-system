namespace Faculty.Api.Features.Professors.v1.IdentityProvisioning;

public static class MarkProfessorIdentityProvisioningSucceeded
{
    public sealed record Command(Guid ProfessorId, Guid IdentityUserId) : ICommand<Result>;

    public sealed class Handler(IProfessorRepository professorRepository, IUnitOfWork unitOfWork, ILogger<Handler> logger)
        : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var professor = await professorRepository.GetByIdAsync(new ProfessorId(request.ProfessorId), cancellationToken);
            if (professor is null)
            {
                logger.LogWarning("Professor identity provisioning succeeded event received, but professor was not found. ProfessorId: {ProfessorId}, IdentityUserId: {IdentityUserId}",
                    request.ProfessorId,
                    request.IdentityUserId);
                return ProfessorErrors.NotFound;
            }
            var markResult = professor.MarkIdentityProvisioningSucceeded(new UserId(request.IdentityUserId));
            if (markResult.IsFailure)
            {
                logger.LogWarning("Failed to mark professor identity provisioning as succeeded. ProfessorId: {ProfessorId}, IdentityUserId: {IdentityUserId}, Error: {Error}",
                    request.ProfessorId,
                    request.IdentityUserId,
                    markResult.Error.Description);
                return markResult;
            }
            await unitOfWork.SaveAsync(cancellationToken);
            logger.LogInformation("Professor identity provisioning marked as succeeded. ProfessorId: {ProfessorId}, IdentityUserId: {IdentityUserId}",
                request.ProfessorId,
                request.IdentityUserId);
            return Result.Success();
        }
    }
}