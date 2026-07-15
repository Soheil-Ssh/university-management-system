namespace Faculty.Api.Features.Department.v1.AssignHead;

public static class AssignHead
{
    public sealed record Request(Guid ProfessorId);

    public sealed record Command(Guid DepartmentId, Guid ProfessorId) : ICommand<Result>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.DepartmentId).NotEmpty();
            RuleFor(x => x.ProfessorId).NotEmpty();
        }
    }

    public sealed class Handler(IDepartmentRepository departmentRepository, IProfessorRepository professorRepository, IUnitOfWork unitOfWork)
        : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var departmentId = new DepartmentId(request.DepartmentId);
            var professorId = new ProfessorId(request.ProfessorId);

            var department = await departmentRepository.GetByIdAsync(departmentId, cancellationToken);
            if (department is null)
                return DepartmentErrors.NotFound;

            if (department.HeadProfessorId == professorId)
                return Result.Success();

            var professor = await professorRepository.GetByIdAsync(professorId, cancellationToken);
            if (professor is null)
                return ProfessorErrors.NotFound;

            var eligibilityResult = professor.EnsureEligibleForAcademicManagement();
            if (eligibilityResult.IsFailure)
                return eligibilityResult.Error;

            var isHeadOfAnotherDepartment = await departmentRepository.IsHeadOfAnotherDepartmentAsync(professorId, departmentId, cancellationToken);
            if (isHeadOfAnotherDepartment)
                return ProfessorErrors.AlreadyHeadOfAnotherDepartment;

            var assignResult = department.AssignHead(professorId);

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
            app.MapPatch("api/v{v:apiVersion}/departments/{departmentId:guid}/head",
                    async (Guid departmentId, Request request, ISender sender, CancellationToken cancellationToken) =>
                    {
                        var command = new Command(departmentId, request.ProfessorId);
                        var result = await sender.Send(command, cancellationToken);
                        return result.ToHttpResult();
                    })
                //.RequirePermission(PermissionCodes.Faculty.DepartmentsAssignHead)
                .Version(app, 1.0)
                .WithName("AssignDepartmentHead")
                .WithTags("Departments");
        }
    }
}