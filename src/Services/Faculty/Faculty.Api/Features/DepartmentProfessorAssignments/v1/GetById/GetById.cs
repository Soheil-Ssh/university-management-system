namespace Faculty.Api.Features.DepartmentProfessorAssignments.v1.GetById;

public static class GetById
{
    public sealed record Response(Guid Id,
        Guid DepartmentId,
        string DepartmentCode,
        string DepartmentName,
        Guid ProfessorId,
        string ProfessorCode,
        string ProfessorFirstName,
        string ProfessorLastName,
        string ProfessorFullName,
        string? Specialization,
        AcademicRank AcademicRank,
        ProfessorEmploymentType EmploymentType,
        bool ProfessorIsActive,
        bool IsDepartmentHead,
        DateTime AssignedAtUtc,
        DateTime? UnassignedAtUtc,
        bool IsActive,
        DateTime CreateAt,
        DateTime UpdateAt);

    public sealed record Query(Guid Id) : IQuery<Result<Response>>;

    public sealed class Handler(FacultyDbContext context)
        : IQueryHandler<Query, Result<Response>>
    {
        public async Task<Result<Response>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var assignmentId = new DepartmentProfessorAssignmentId(request.Id);

            var response = await (
                    from assignment in context.DepartmentProfessorAssignments.AsNoTracking()
                    join department in context.Departments.AsNoTracking()
                        on assignment.DepartmentId equals department.Id
                    join professor in context.Professors.AsNoTracking()
                        on assignment.ProfessorId equals professor.Id
                    where assignment.Id == assignmentId
                    select new Response(
                        assignment.Id.Value,
                        department.Id.Value,
                        department.Code.Value,
                        department.Name,
                        professor.Id.Value,
                        professor.Code.Value,
                        professor.FirstName.Value,
                        professor.LastName.Value,
                        professor.FullName,
                        professor.Specialization,
                        professor.AcademicRank,
                        professor.EmploymentType,
                        professor.IsActive,
                        department.HeadProfessorId == professor.Id,
                        assignment.AssignedAt,
                        assignment.UnassignedAt,
                        assignment.UnassignedAt == null,
                        assignment.CreatedAt,
                        assignment.UpdatedAt))
                .FirstOrDefaultAsync(cancellationToken);

            if (response is null)
                return DepartmentProfessorAssignmentErrors.NotFound;

            return response;
        }
    }

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet(
                    "api/v{v:apiVersion}/department-professor-assignments/{id:guid}",
                    async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    {
                        var result = await sender.Send(new Query(id), cancellationToken);
                        return result.ToHttpResult();
                    })
                //.RequirePermission(PermissionCodes.Faculty.DepartmentProfessorsRead)
                .Version(app, 1.0)
                .WithName("GetDepartmentProfessorAssignmentById")
                .WithTags("Department Professor Assignments");
        }
    }
}