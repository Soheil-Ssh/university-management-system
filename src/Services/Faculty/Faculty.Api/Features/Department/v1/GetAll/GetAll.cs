using SharedKernel.Abstractions.Pagination;
using SharedKernel.Persistence.Extensions;

namespace Faculty.Api.Features.Department.v1.GetAll;

public static class GetAll
{
    public enum DepartmentSortBy
    {
        Code = 1,
        Name = 2,
        CreatedAt = 3,
        UpdatedAt = 4
    }

    public enum SortDirection
    {
        Asc = 1,
        Desc = 2
    }

    public sealed record GetAllDepartmentsRequest(Guid? FacultyId,
        string? Code,
        string? Name,
        bool? IsActive,
        DateTime? FromCreatedAt,
        DateTime? ToCreatedAt,
        DateTime? FromUpdatedAt,
        DateTime? ToUpdatedAt,
        DepartmentSortBy SortBy = DepartmentSortBy.CreatedAt,
        SortDirection SortDirection = SortDirection.Desc,
        int Page = 1,
        int PageSize = 20);

    public sealed record Response(Guid Id,
        string Code,
        string Name,
        string? ShortName,
        Guid FacultyId,
        string FacultyName,
        bool HasHeadProfessor,
        bool HasPrimaryExpert,
        bool IsActive,
        DateTime CreatedAt);

    public sealed record Query(Guid? FacultyId,
        string? Code,
        string? Name,
        bool? IsActive,
        DateTime? FromCreatedAt,
        DateTime? ToCreatedAt,
        DateTime? FromUpdatedAt,
        DateTime? ToUpdatedAt,
        DepartmentSortBy SortBy = DepartmentSortBy.CreatedAt,
        SortDirection SortDirection = SortDirection.Desc,
        int Page = 1,
        int PageSize = 20) : IQuery<Result<PagedResult<Response>>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.FacultyId).NotEmpty().When(x => x.FacultyId.HasValue);
            RuleFor(x => x.Code).MaximumLength(50);
            RuleFor(x => x.Name).MaximumLength(150);
            RuleFor(x => x.SortBy).IsInEnum();
            RuleFor(x => x.SortDirection).IsInEnum();
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
            RuleFor(x => x)
                .Must(x => !x.FromCreatedAt.HasValue ||
                           !x.ToCreatedAt.HasValue ||
                           x.FromCreatedAt <= x.ToCreatedAt)
                .WithMessage("FromCreatedAt cannot be greater than ToCreatedAt.");
            RuleFor(x => x)
                .Must(x => !x.FromUpdatedAt.HasValue ||
                           !x.ToUpdatedAt.HasValue ||
                           x.FromUpdatedAt <= x.ToUpdatedAt)
                .WithMessage("FromUpdatedAt cannot be greater than ToUpdatedAt.");
        }
    }

    public sealed class Handler(FacultyDbContext context) : IQueryHandler<Query, Result<PagedResult<Response>>>
    {
        public async Task<Result<PagedResult<Response>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = context.Departments.AsNoTracking().AsQueryable();

            if (request.FacultyId.HasValue)
                query = query.Where(r => r.FacultyId == new FacultyId(request.FacultyId.Value));

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                var codeResult = DepartmentCode.FromString(request.Code);
                if (codeResult.IsFailure)
                    return codeResult.Error;

                query = query.Where(department => department.Code == codeResult.Data);
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var name = request.Name.Trim();

                query = query.Where(department => EF.Functions.ILike(department.Name, $"%{name}%"));
            }

            if (request.IsActive.HasValue)
                query = query.Where(department => department.IsActive == request.IsActive.Value);

            if (request.FromCreatedAt.HasValue)
                query = query.Where(d => d.CreatedAt >= request.FromCreatedAt.Value);

            if (request.ToCreatedAt.HasValue)
                query = query.Where(d => d.CreatedAt <= request.ToCreatedAt.Value);

            if (request.FromUpdatedAt.HasValue)
                query = query.Where(d => d.UpdatedAt >= request.FromUpdatedAt.Value);

            if (request.ToUpdatedAt.HasValue)
            {
                query = query.Where(d => d.UpdatedAt <= request.ToUpdatedAt.Value);
            }

            query = ApplySorting(query, request.SortBy, request.SortDirection);

            return await query
                .Join(
                    context.Faculties.AsNoTracking(),
                    department => department.FacultyId,
                    faculty => faculty.Id,
                    (department, faculty) => new Response(
                        department.Id.Value,
                        department.Code.Value,
                        department.Name,
                        department.ShortName,
                        department.FacultyId.Value,
                        faculty.Name,
                        department.HeadProfessorId != null,
                        department.PrimaryExpertEmployeeId != null,
                        department.IsActive,
                        department.CreatedAt))
                .ToPagedResultAsync(
                    request.Page,
                    request.PageSize,
                    cancellationToken);
        }

        private static IQueryable<Domain.Department.Department> ApplySorting(IQueryable<Domain.Department.Department> query, DepartmentSortBy sortBy, SortDirection direction)
            => (sortBy, direction) switch
            {
                (DepartmentSortBy.Code, SortDirection.Asc) =>
                    query.OrderBy(x => x.Code).ThenBy(x => x.Id),

                (DepartmentSortBy.Code, SortDirection.Desc) =>
                    query.OrderByDescending(x => x.Code).ThenBy(x => x.Id),

                (DepartmentSortBy.Name, SortDirection.Asc) =>
                    query.OrderBy(x => x.Name).ThenBy(x => x.Id),

                (DepartmentSortBy.Name, SortDirection.Desc) =>
                    query.OrderByDescending(x => x.Name).ThenBy(x => x.Id),

                (DepartmentSortBy.UpdatedAt, SortDirection.Asc) =>
                    query.OrderBy(x => x.UpdatedAt).ThenBy(x => x.Id),

                (DepartmentSortBy.UpdatedAt, SortDirection.Desc) =>
                    query.OrderByDescending(x => x.UpdatedAt).ThenBy(x => x.Id),

                (DepartmentSortBy.CreatedAt, SortDirection.Asc) =>
                    query.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id),

                _ => query.OrderByDescending(x => x.CreatedAt).ThenBy(x => x.Id)
            };
    }

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v{v:apiVersion}/departments",
                    async ([AsParameters] GetAllDepartmentsRequest request, ISender sender, CancellationToken cancellationToken) =>
                    {
                        var query = request.Adapt<Query>();
                        var result = await sender.Send(query, cancellationToken);
                        return result.ToHttpResult();
                    })
                //.RequirePermission(PermissionCodes.Faculty.DepartmentsRead)
                .Version(app, 1.0)
                .WithName("GetAllDepartments")
                .WithTags("Departments");
        }
    }
}