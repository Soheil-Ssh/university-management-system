namespace Faculty.Api.Features.DepartmentProfessorAssignments.v1.AssignProfessorToDepartment;

public static class AssignProfessorToDepartment
{
    public sealed record AssignProfessorToDepartmentRequest(Guid ProfessorId);

    public sealed record Command(Guid DepartmentId, Guid ProfessorId) : ICommand<Result<Guid>>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.DepartmentId).NotEmpty();
            RuleFor(x => x.ProfessorId).NotEmpty();
        }
    }

    public sealed class Handler(IDepartmentRepository departmentRepository,
        IProfessorRepository professorRepository,
        IDepartmentProfessorAssignmentRepository assignmentRepository,
        IUnitOfWork unitOfWork) : ICommandHandler<Command, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
        {
            var departmentId = new DepartmentId(request.DepartmentId);
            var professorId = new ProfessorId(request.ProfessorId);

            var department = await departmentRepository.GetByIdAsync(departmentId, cancellationToken);
            if (department is null)
                return DepartmentErrors.NotFound;
            if (!department.IsActive)
                return DepartmentErrors.Inactive;

            var professor = await professorRepository.GetByIdAsync(professorId, cancellationToken);
            if (professor is null)
                return ProfessorErrors.NotFound;
            if (!professor.IsActive)
                return ProfessorErrors.Inactive;

            var assignmentExists = await assignmentRepository.ExistsActiveAsync(departmentId, professorId, cancellationToken);
            if (assignmentExists)
                return DepartmentProfessorAssignmentErrors.AlreadyExists;

            var assignmentResult = DepartmentProfessorAssignment.Create(departmentId, professorId);
            if (assignmentResult.IsFailure)
                return assignmentResult.Error;

            await assignmentRepository.AddAsync(assignmentResult.Data, cancellationToken);
            await unitOfWork.SaveAsync(cancellationToken);

            return assignmentResult.Data.Id.Value;
        }
    }

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/v{v:apiVersion}/departments/{departmentId:guid}/professors",
                    async (ISender sender, Guid departmentId, AssignProfessorToDepartmentRequest request, CancellationToken cancellationToken) =>
                    {
                        var command = new Command(departmentId, request.ProfessorId);
                        var result = await sender.Send(command, cancellationToken);
                        return result.ToHttpResult();
                    })
                .Version(app, 1.0)
                //.RequireAuthorization()
                .WithName("AssignProfessorToDepartment")
                .WithTags("Department Professors");
        }
    }
}