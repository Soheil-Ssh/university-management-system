namespace Faculty.Api.Features.Professors.v1.IdentityDeactivation;

public static class MarkProfessorIdentityDeactivationFailed
{
    public sealed record Command(Guid ProfessorId, string Reason) : ICommand<Result>;

    public sealed class Handler(IProfessorRepository professorRepository, IUnitOfWork unitOfWork)
        : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var professor = await professorRepository.GetByIdAsync(new ProfessorId(request.ProfessorId), cancellationToken);
            if (professor is null)
                return ProfessorErrors.NotFound;

            var result = professor.MarkIdentityDeactivationFailed(request.Reason);
            if (result.IsFailure)
                return result.Error;

            await unitOfWork.SaveAsync(cancellationToken);
            return Result.Success();
        }
    }
}