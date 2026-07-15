namespace Faculty.Api.Features.Professors.v1.Activate;

public static class Activate
{
    public sealed record Command(Guid ProfessorId) : ICommand<Result>;

    public sealed class Handler(IProfessorRepository professorRepository, IUnitOfWork unitOfWork) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var professor = await professorRepository.GetByIdAsync(new ProfessorId(request.ProfessorId), cancellationToken);
            if (professor is null)
                return ProfessorErrors.NotFound;

            var activateResult = professor.Activate();

            if (activateResult.IsFailure)
                return activateResult.Error;

            await unitOfWork.SaveAsync(cancellationToken);
            return Result.Success();
        }
    }

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("api/v{v:apiVersion}/professors/{professorId:guid}/activate",
                    async (Guid professorId, ISender sender, CancellationToken cancellationToken) =>
                    {
                        var result = await sender.Send(new Command(professorId), cancellationToken);
                        return result.ToHttpResult();
                    })
                //.RequirePermission(PermissionCodes.Faculty.ProfessorsActivate)
                .Version(app, 1.0)
                .WithName("ActivateProfessor")
                .WithTags("Professors");
        }
    }
}