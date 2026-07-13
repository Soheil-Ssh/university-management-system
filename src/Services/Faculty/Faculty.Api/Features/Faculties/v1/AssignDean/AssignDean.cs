namespace Faculty.Api.Features.Faculties.v1.AssignDean;

public static class AssignDean
{
    public sealed record Request(Guid ProfessorId);

    public sealed record Command(Guid FacultyId, Guid ProfessorId) : ICommand<Result>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.FacultyId).NotEmpty();
            RuleFor(x => x.ProfessorId).NotEmpty();
        }
    }

    public sealed class Handler(IFacultyRepository facultyRepository, 
        IProfessorRepository professorRepository, IUnitOfWork unitOfWork) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var facultyId = new FacultyId(request.FacultyId);
            var professorId = new ProfessorId(request.ProfessorId);

            var faculty = await facultyRepository.GetByIdAsync(facultyId, cancellationToken);
            if (faculty is null)
                return FacultyErrors.NotFound;

            var professor = await professorRepository.GetByIdAsync(professorId, cancellationToken);
            if (professor is null)
                return ProfessorErrors.NotFound;

            if (!professor.IsActive)
                return ProfessorErrors.InactiveForDeanAssignment;

            var eligibilityResult = professor.EnsureEligibleForDeanAssignment();
            if (eligibilityResult.IsFailure)
                return eligibilityResult.Error;

            var isDeanOfAnotherFaculty = await facultyRepository.IsDeanOfAnotherFacultyAsync(professorId, facultyId, cancellationToken);
            if (isDeanOfAnotherFaculty)
                return ProfessorErrors.AlreadyDeanOfAnotherFaculty;

            var assignResult = faculty.AssignDean(professorId);
            if (assignResult.IsFailure)
                return assignResult.Error;

            await unitOfWork.SaveAsync(cancellationToken);
            return Result.Success();
        }
    }

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPatch("api/v{v:apiVersion}/faculties/{facultyId:guid}/dean", 
                    async (Guid facultyId, Request request, ISender sender, CancellationToken cancellationToken) =>
                    {
                        var command = new Command(facultyId, request.ProfessorId);
                        var result = await sender.Send(command, cancellationToken);
                        return result.ToHttpResult();
                    })
                //.RequirePermission(PermissionCodes.Faculty.FacultiesAssignDean)
                .Version(app, 1.0)
                .WithName("AssignFacultyDean")
                .WithTags("Faculties");
        }
    }
}