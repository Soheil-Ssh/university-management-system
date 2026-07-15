namespace Faculty.Api.Features.Professors.v1.Deactivate;

public static class Deactivate
{
    public sealed record Command(Guid ProfessorId) : ICommand<Result>;

    public sealed class Handler(IProfessorRepository professorRepository, IUnitOfWork unitOfWork) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var professor = await professorRepository.GetByIdAsync(new ProfessorId(request.ProfessorId), cancellationToken);
            if (professor is null)
                return ProfessorErrors.NotFound;

            var deactivateResult = professor.Deactivate();

            if (deactivateResult.IsFailure)
                return deactivateResult.Error;

            await unitOfWork.SaveAsync(cancellationToken);

            return Result.Success();
        }
    }

    public sealed class DomainEventHandler(IFacultyRepository facultyRepository, IDepartmentRepository departmentRepository)
        : IEventHandler<ProfessorDeactivatedDomainEvent>
    {
        public async Task Handle(ProfessorDeactivatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var professorId = notification.ProfessorId;

            var faculty = await facultyRepository.GetByDeanProfessorIdAsync(professorId, cancellationToken);
            if (faculty is not null)
            {
                var removeDeanResult = faculty.RemoveDean();
                if (removeDeanResult.IsFailure)
                    throw new InvalidOperationException(removeDeanResult.Error.ToString());
            }

            var department = await departmentRepository.GetByHeadProfessorIdAsync(professorId, cancellationToken);
            if (department is not null)
            {
                var removeHeadResult = department.RemoveHead();
                if (removeHeadResult.IsFailure)
                    throw new InvalidOperationException(removeHeadResult.Error.ToString());
            }
        }
    }

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("api/v{v:apiVersion}/professors/{professorId:guid}/deactivate",
                    async (Guid professorId, ISender sender, CancellationToken cancellationToken) =>
                    {
                        var result = await sender.Send(new Command(professorId), cancellationToken);
                        return result.ToHttpResult();
                    })
                //.RequirePermission(PermissionCodes.Faculty.ProfessorsDeactivate)
                .Version(app, 1.0)
                .WithName("DeactivateProfessor")
                .WithTags("Professors");
        }
    }
}