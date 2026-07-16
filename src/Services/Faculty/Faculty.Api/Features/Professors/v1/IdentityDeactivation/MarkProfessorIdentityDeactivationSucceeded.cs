namespace Faculty.Api.Features.Professors.v1.IdentityDeactivation;

public static class MarkProfessorIdentityDeactivationSucceeded
{
    public sealed record Command(Guid ProfessorId) : ICommand<Result>;

    public sealed class Handler(IProfessorRepository professorRepository, IUnitOfWork unitOfWork)
        : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var professor = await professorRepository.GetByIdAsync(new ProfessorId(request.ProfessorId), cancellationToken);
            if (professor is null)
                return ProfessorErrors.NotFound;

            var result = professor.MarkIdentityDeactivationSucceeded();
            if (result.IsFailure)
                return result.Error;

            await unitOfWork.SaveAsync(cancellationToken);
            return Result.Success();
        }
    }
}