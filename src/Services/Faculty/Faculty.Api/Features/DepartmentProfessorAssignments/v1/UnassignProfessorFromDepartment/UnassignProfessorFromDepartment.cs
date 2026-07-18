namespace Faculty.Api.Features.DepartmentProfessorAssignments.v1.UnassignProfessorFromDepartment;

public static class UnassignProfessorFromDepartment
{
    public sealed record Command(Guid DepartmentId, Guid ProfessorId) : ICommand<Result>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.DepartmentId).NotEmpty();
            RuleFor(x => x.ProfessorId).NotEmpty();
        }
    }

    public sealed class Handler(IDepartmentRepository departmentRepository,
        IDepartmentProfessorAssignmentRepository assignmentRepository,
        IUnitOfWork unitOfWork)
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
                return DepartmentProfessorAssignmentErrors.DepartmentHeadCannotBeUnassigned;

            var assignment = await assignmentRepository.GetActiveAsync(departmentId, professorId, cancellationToken);
            if (assignment is null)
                return Result.Success();

            var unassignResult = assignment.Unassign();
            if (unassignResult.IsFailure)
                return unassignResult.Error;

            await unitOfWork.SaveAsync(cancellationToken);

            return Result.Success();
        }
    }

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/v{v:apiVersion}/departments/{departmentId:guid}/professors/{professorId:guid}",
                    async (ISender sender, Guid departmentId, Guid professorId, CancellationToken cancellationToken) =>
                    {
                        var command = new Command(departmentId, professorId);
                        var result = await sender.Send(command, cancellationToken);
                        return result.ToHttpResult();
                    })
                .Version(app, 1.0)
                //.RequireAuthorization()
                .WithName("UnassignProfessorFromDepartment")
                .WithTags("Department Professors");
        }
    }
}