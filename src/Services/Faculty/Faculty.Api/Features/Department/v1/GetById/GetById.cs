namespace Faculty.Api.Features.Department.v1.GetById;

public static class GetById
{
    public sealed record FacultyResponse(Guid Id, string Code, string Name);

    public sealed record HeadProfessorResponse(Guid Id, string Code, string FullName, AcademicRank AcademicRank, bool IsActive);

    public sealed record Response(Guid Id,
        string Code,
        string Name,
        string? ShortName,
        string? Description,
        FacultyResponse Faculty,
        HeadProfessorResponse? HeadProfessor,
        Guid? PrimaryExpertEmployeeId,
        string? Email,
        string? PhoneNumber,
        string? InternalPhoneNumber,
        string? OfficeLocation,
        bool IsActive,
        DateTime CreatedAt,
        DateTime UpdatedAt);

    public sealed record Query(Guid Id) : IQuery<Result<Response>>;

    public sealed class Handler(FacultyDbContext context) : IQueryHandler<Query, Result<Response>>
    {
        public async Task<Result<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var departmentId = new DepartmentId(request.Id);

            var department = await context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == departmentId, cancellationToken);
            if (department is null)
                return DepartmentErrors.NotFound;

            var faculty = await context.Faculties
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == department.FacultyId, cancellationToken);
            if (faculty is null)
                return FacultyErrors.NotFound;

            HeadProfessorResponse? headProfessorResponse = null;
            if (department.HeadProfessorId is not null)
            {
                headProfessorResponse = await context.Professors
                    .AsNoTracking()
                    .Select(p => new HeadProfessorResponse(p.Id.Value, p.Code.Value, p.FullName, p.AcademicRank, p.IsActive))
                    .FirstAsync(p => p.Id == department.HeadProfessorId.Value, cancellationToken);
            }

            return new Response(
                department.Id.Value,
                department.Code.Value,
                department.Name,
                department.ShortName,
                department.Description,
                new FacultyResponse(faculty.Id.Value, faculty.Code.Value, faculty.Name),
                headProfessorResponse,
                department.PrimaryExpertEmployeeId?.Value,
                department.Email?.Value,
                department.PhoneNumber?.Value,
                department.InternalPhoneNumber,
                department.OfficeLocation,
                department.IsActive,
                department.CreatedAt,
                department.UpdatedAt);
        }
    }

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v{v:apiVersion}/departments/{id:guid}",
                    async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    {
                        var query = new Query(id);
                        var result = await sender.Send(query, cancellationToken);
                        return result.ToHttpResult();
                    })
                //.RequirePermission(PermissionCodes.Faculty.DepartmentsRead)
                .Version(app, 1.0)
                .WithName("GetDepartmentById")
                .WithTags("Departments");
        }
    }
}