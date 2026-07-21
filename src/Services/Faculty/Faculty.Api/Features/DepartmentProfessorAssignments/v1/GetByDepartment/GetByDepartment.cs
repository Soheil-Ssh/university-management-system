using SharedKernel.Abstractions.Pagination;
using SharedKernel.Persistence.Extensions;

namespace Faculty.Api.Features.DepartmentProfessorAssignments.v1.GetByDepartment;

public static class GetByDepartment
{
    public enum DepartmentProfessorSortBy
    {
        AssignedAt = 1,
        Code = 2,
        FirstName = 3,
        LastName = 4,
        AcademicRank = 5
    }

    public enum SortDirection
    {
        Asc = 1,
        Desc = 2
    }

    public sealed record GetDepartmentProfessorsRequest(
        string? Code,
        string? Name,
        string? Specialization,
        AcademicRank? AcademicRank,
        ProfessorEmploymentType? EmploymentType,
        bool? IsActive,
        DepartmentProfessorSortBy DepartmentProfessorSortBy = DepartmentProfessorSortBy.AssignedAt,
        SortDirection SortDirection = SortDirection.Desc,
        int Page = 1,
        int PageSize = 20);

    public sealed record Response(
        Guid AssignmentId,
        Guid ProfessorId,
        string Code,
        string FirstName,
        string LastName,
        string FullName,
        string? Specialization,
        AcademicRank AcademicRank,
        ProfessorEmploymentType EmploymentType,
        bool IsDepartmentHead,
        bool IsActive,
        DateTime AssignedAtUtc);

    public sealed record Query(
        Guid DepartmentId,
        string? Code,
        string? Name,
        string? Specialization,
        AcademicRank? AcademicRank,
        ProfessorEmploymentType? EmploymentType,
        bool? IsActive,
        DepartmentProfessorSortBy DepartmentProfessorSortBy = DepartmentProfessorSortBy.AssignedAt,
        SortDirection SortDirection = SortDirection.Desc,
        int Page = 1,
        int PageSize = 20) : IQuery<Result<PagedResult<Response>>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.DepartmentId).NotEmpty();
            RuleFor(x => x.Code).MaximumLength(50);
            RuleFor(x => x.Name).MaximumLength(150);
            RuleFor(x => x.Specialization).MaximumLength(150);
            RuleFor(x => x.AcademicRank).IsInEnum().When(x => x.AcademicRank.HasValue);
            RuleFor(x => x.EmploymentType).IsInEnum().When(x => x.EmploymentType.HasValue);
            RuleFor(x => x.DepartmentProfessorSortBy).IsInEnum();
            RuleFor(x => x.SortDirection).IsInEnum();
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        }
    }

    public sealed class Handler(FacultyDbContext context) : IQueryHandler<Query, Result<PagedResult<Response>>>
    {
        public async Task<Result<PagedResult<Response>>> Handle(Query request,
            CancellationToken cancellationToken)
        {
            var departmentId = new DepartmentId(request.DepartmentId);

            var department = await context.Departments
                .AsNoTracking()
                .Where(d => d.Id == departmentId)
                .Select(d => new
                {
                    d.HeadProfessorId
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (department is null)
                return DepartmentErrors.NotFound;

            var query =
                from assignment in context.DepartmentProfessorAssignments.AsNoTracking()
                join professor in context.Professors.AsNoTracking()
                    on assignment.ProfessorId equals professor.Id
                where assignment.DepartmentId == departmentId &&
                      assignment.UnassignedAt == null
                select new
                {
                    Assignment = assignment,
                    Professor = professor
                };

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                var codeResult = ProfessorCode.FromString(request.Code);
                if (codeResult.IsFailure)
                    return codeResult.Error;

                query = query.Where(x => x.Professor.Code == codeResult.Data);
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var name = request.Name.Trim();
                query = query.Where(x =>
                    EF.Functions.ILike(x.Professor.FirstName.Value, $"%{name}%") ||
                    EF.Functions.ILike(x.Professor.LastName.Value, $"%{name}%"));
            }

            if (!string.IsNullOrWhiteSpace(request.Specialization))
            {
                var specialization = request.Specialization.Trim();
                query = query.Where(x => EF.Functions.ILike(x.Professor.Specialization, $"%{specialization}%"));
            }

            if (request.AcademicRank.HasValue)
                query = query.Where(x => x.Professor.AcademicRank == request.AcademicRank.Value);

            if (request.EmploymentType.HasValue)
                query = query.Where(x => x.Professor.EmploymentType == request.EmploymentType.Value);

            if (request.IsActive.HasValue)
                query = query.Where(x => x.Professor.IsActive == request.IsActive.Value);

            query = (SortBy: request.DepartmentProfessorSortBy, request.SortDirection) switch
            {
                (DepartmentProfessorSortBy.Code, SortDirection.Asc) =>
                    query.OrderBy(x => x.Professor.Code).ThenBy(x => x.Professor.Id),

                (DepartmentProfessorSortBy.Code, SortDirection.Desc) =>
                    query.OrderByDescending(x => x.Professor.Code).ThenBy(x => x.Professor.Id),

                (DepartmentProfessorSortBy.FirstName, SortDirection.Asc) =>
                    query.OrderBy(x => x.Professor.FirstName).ThenBy(x => x.Professor.Id),

                (DepartmentProfessorSortBy.FirstName, SortDirection.Desc) =>
                    query.OrderByDescending(x => x.Professor.FirstName).ThenBy(x => x.Professor.Id),

                (DepartmentProfessorSortBy.LastName, SortDirection.Asc) =>
                    query.OrderBy(x => x.Professor.LastName).ThenBy(x => x.Professor.Id),

                (DepartmentProfessorSortBy.LastName, SortDirection.Desc) =>
                    query.OrderByDescending(x => x.Professor.LastName).ThenBy(x => x.Professor.Id),

                (DepartmentProfessorSortBy.AcademicRank, SortDirection.Asc) =>
                    query.OrderBy(x => x.Professor.AcademicRank).ThenBy(x => x.Professor.Id),

                (DepartmentProfessorSortBy.AcademicRank, SortDirection.Desc) =>
                    query.OrderByDescending(x => x.Professor.AcademicRank).ThenBy(x => x.Professor.Id),

                (DepartmentProfessorSortBy.AssignedAt, SortDirection.Asc) =>
                    query.OrderBy(x => x.Assignment.AssignedAt).ThenBy(x => x.Professor.Id),

                _ => query.OrderByDescending(x => x.Assignment.AssignedAt)
                    .ThenBy(x => x.Professor.Id)
            };

            return await query
                .Select(x => new Response(
                    x.Assignment.Id.Value,
                    x.Professor.Id.Value,
                    x.Professor.Code.Value,
                    x.Professor.FirstName.Value,
                    x.Professor.LastName.Value,
                    x.Professor.FullName,
                    x.Professor.Specialization,
                    x.Professor.AcademicRank,
                    x.Professor.EmploymentType,
                    department.HeadProfessorId == x.Professor.Id,
                    x.Professor.IsActive,
                    x.Assignment.AssignedAt))
                .ToPagedResultAsync(request.Page, request.PageSize, cancellationToken);
        }
    }

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v{v:apiVersion}/departments/{departmentId:guid}/professors",
                    async (Guid departmentId, [AsParameters] GetDepartmentProfessorsRequest request, ISender sender, CancellationToken cancellationToken) =>
                    {
                        var query = new Query(departmentId,
                            request.Code,
                            request.Name,
                            request.Specialization,
                            request.AcademicRank,
                            request.EmploymentType,
                            request.IsActive,
                            request.DepartmentProfessorSortBy,
                            request.SortDirection,
                            request.Page,
                            request.PageSize);

                        var result = await sender.Send(query, cancellationToken);
                        return result.ToHttpResult();
                    })
                //.RequirePermission(PermissionCodes.Faculty.DepartmentProfessorsRead)
                .Version(app, 1.0)
                .WithName("GetDepartmentProfessors")
                .WithTags("Department Professors");
        }
    }
}