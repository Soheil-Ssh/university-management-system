using SharedKernel.Abstractions.Pagination;
using SharedKernel.Persistence.Extensions;

namespace Faculty.Api.Features.DepartmentProfessorAssignments.v1.GetHistory;

public static class GetHistory
{
    public enum AssignmentSortBy
    {
        AssignedAt = 1,
        UnassignedAt = 2
    }

    public enum SortDirection
    {
        Asc = 1,
        Desc = 2
    }

    public sealed record GetAssignmentHistoryRequest(Guid? DepartmentId,
        Guid? ProfessorId,
        bool? IsActive,
        DateTime? FromAssignedAt,
        DateTime? ToAssignedAt,
        DateTime? FromUnassignedAt,
        DateTime? ToUnassignedAt,
        AssignmentSortBy SortBy = AssignmentSortBy.AssignedAt,
        SortDirection SortDirection = SortDirection.Desc,
        int Page = 1,
        int PageSize = 20);

    public sealed record Response(Guid Id,
        Guid DepartmentId,
        string DepartmentCode,
        string DepartmentName,
        Guid ProfessorId,
        string ProfessorCode,
        string ProfessorFullName,
        DateTime AssignedAtUtc,
        DateTime? UnassignedAt,
        bool IsActive);

    public sealed record Query(Guid? DepartmentId,
        Guid? ProfessorId,
        bool? IsActive,
        DateTime? FromAssignedAt,
        DateTime? ToAssignedAt,
        DateTime? FromUnassignedAt,
        DateTime? ToUnassignedAt,
        AssignmentSortBy SortBy = AssignmentSortBy.AssignedAt,
        SortDirection SortDirection = SortDirection.Desc,
        int Page = 1,
        int PageSize = 20) : IQuery<Result<PagedResult<Response>>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.DepartmentId).NotEmpty().When(x => x.DepartmentId.HasValue);
            RuleFor(x => x.ProfessorId).NotEmpty().When(x => x.ProfessorId.HasValue);
            RuleFor(x => x.SortBy).IsInEnum();
            RuleFor(x => x.SortDirection).IsInEnum();
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);

            RuleFor(x => x)
                .Must(x => !x.FromAssignedAt.HasValue ||
                           !x.ToAssignedAt.HasValue ||
                           x.FromAssignedAt <= x.ToAssignedAt)
                .WithMessage("FromAssignedAt cannot be greater than ToAssignedAt.");

            RuleFor(x => x)
                .Must(x => !x.FromUnassignedAt.HasValue ||
                           !x.ToUnassignedAt.HasValue ||
                           x.FromUnassignedAt <= x.ToUnassignedAt)
                .WithMessage("FromUnassignedAt cannot be greater than ToUnassignedAt.");
        }
    }

    public sealed class Handler(FacultyDbContext context)
       : IQueryHandler<Query, Result<PagedResult<Response>>>
    {
        public async Task<Result<PagedResult<Response>>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var assignments = context.DepartmentProfessorAssignments
                .AsNoTracking()
                .AsQueryable();

            if (request.DepartmentId.HasValue)
                assignments = assignments.Where(x =>
                    x.DepartmentId == new DepartmentId(request.DepartmentId.Value));

            if (request.ProfessorId.HasValue)
                assignments = assignments.Where(x =>
                    x.ProfessorId == new ProfessorId(request.ProfessorId.Value));

            if (request.IsActive.HasValue)
            {
                assignments = request.IsActive.Value
                    ? assignments.Where(x => x.UnassignedAt == null)
                    : assignments.Where(x => x.UnassignedAt != null);
            }

            if (request.FromAssignedAt.HasValue)
                assignments = assignments.Where(x => x.AssignedAt >= request.FromAssignedAt.Value);

            if (request.ToAssignedAt.HasValue)
                assignments = assignments.Where(x => x.AssignedAt <= request.ToAssignedAt.Value);

            if (request.FromUnassignedAt.HasValue)
                assignments = assignments.Where(x => x.UnassignedAt >= request.FromUnassignedAt.Value);

            if (request.ToUnassignedAt.HasValue)
                assignments = assignments.Where(x => x.UnassignedAt <= request.ToUnassignedAt.Value);

            assignments = ApplySorting(assignments, request.SortBy, request.SortDirection);

            var query =
                from assignment in assignments
                join department in context.Departments.AsNoTracking()
                    on assignment.DepartmentId equals department.Id
                join professor in context.Professors.AsNoTracking()
                    on assignment.ProfessorId equals professor.Id
                select new Response(
                    assignment.Id.Value,
                    department.Id.Value,
                    department.Code.Value,
                    department.Name,
                    professor.Id.Value,
                    professor.Code.Value,
                    professor.FullName,
                    assignment.AssignedAt,
                    assignment.UnassignedAt,
                    assignment.UnassignedAt == null);

            return await query.ToPagedResultAsync(
                request.Page,
                request.PageSize,
                cancellationToken);
        }

        private static IQueryable<DepartmentProfessorAssignment> ApplySorting(IQueryable<DepartmentProfessorAssignment> query, AssignmentSortBy sortBy, SortDirection direction)
            => (sortBy, direction) switch
            {
                (AssignmentSortBy.UnassignedAt, SortDirection.Asc) =>
                    query.OrderBy(x => x.UnassignedAt).ThenBy(x => x.Id),

                (AssignmentSortBy.UnassignedAt, SortDirection.Desc) =>
                    query.OrderByDescending(x => x.UnassignedAt).ThenBy(x => x.Id),

                (AssignmentSortBy.AssignedAt, SortDirection.Asc) =>
                    query.OrderBy(x => x.AssignedAt).ThenBy(x => x.Id),

                _ => query.OrderByDescending(x => x.AssignedAt).ThenBy(x => x.Id)
            };
    }

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v{v:apiVersion}/department-professor-assignments/history",
                    async ([AsParameters] GetAssignmentHistoryRequest request, ISender sender, CancellationToken cancellationToken) =>
                    {
                        var query = request.Adapt<Query>();
                        var result = await sender.Send(query, cancellationToken);
                        return result.ToHttpResult();
                    })
                //.RequirePermission(PermissionCodes.Faculty.DepartmentProfessorsRead)
                .Version(app, 1.0)
                .WithName("GetDepartmentProfessorAssignmentHistory")
                .WithTags("Department Professor Assignments");
        }
    }
}