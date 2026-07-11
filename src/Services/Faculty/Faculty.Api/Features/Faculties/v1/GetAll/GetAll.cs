using SharedKernel.Api.Contracts;

namespace Faculty.Api.Features.Faculties.v1.GetAll;

public class GetAll
{
    public enum FacultySortBy
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

    public sealed record GetAllFacultiesRequest(
        string? Code,
        string? Name,
        bool? IsActive,
        DateTime? FromCreatedAt,
        DateTime? ToCreatedAt,
        DateTime? FromUpdatedAt,
        DateTime? ToUpdatedAt,
        FacultySortBy SortBy = FacultySortBy.CreatedAt,
        SortDirection SortDirection = SortDirection.Desc,
        int Page = 1,
        int PageSize = 20);

    public sealed record Response(
        Guid Id,
        string Code,
        string Name,
        string? Description,
        bool IsActive,
        DateTime CreatedAt,
        DateTime UpdatedAt);

    public sealed record Query(
        string? Code,
        string? Name,
        bool? IsActive,
        DateTime? FromCreatedAt,
        DateTime? ToCreatedAt,
        DateTime? FromUpdatedAt,
        DateTime? ToUpdatedAt,
        FacultySortBy SortBy = FacultySortBy.CreatedAt,
        SortDirection SortDirection = SortDirection.Desc,
        int Page = 1,
        int PageSize = 20)
        : IQuery<Result<PagedResult<Response>>>;

    public sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
            RuleFor(x => x.SortBy).IsInEnum();
            RuleFor(x => x.SortDirection).IsInEnum();
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

    public sealed class Handler(FacultyDbContext context)
        : IQueryHandler<Query, Result<PagedResult<Response>>>
    {
        public async Task<Result<PagedResult<Response>>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var query = context.Faculties
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                var codeResult = FacultyCode.FromString(request.Code);

                if (codeResult.IsFailure)
                    return codeResult.Error;

                query = query.Where(f => f.Code == codeResult.Data);
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var name = request.Name.Trim();

                query = query.Where(f =>
                    EF.Functions.ILike(f.Name, $"%{name}%"));
            }

            if (request.IsActive.HasValue)
                query = query.Where(f => f.IsActive == request.IsActive.Value);

            if (request.FromCreatedAt.HasValue)
                query = query.Where(f => f.CreatedAt >= request.FromCreatedAt.Value);

            if (request.ToCreatedAt.HasValue)
                query = query.Where(f => f.CreatedAt <= request.ToCreatedAt.Value);

            if (request.FromUpdatedAt.HasValue)
                query = query.Where(f => f.UpdatedAt >= request.FromUpdatedAt.Value);

            if (request.ToUpdatedAt.HasValue)
                query = query.Where(f => f.UpdatedAt <= request.ToUpdatedAt.Value);

            query = ApplySorting(query, request.SortBy, request.SortDirection);

            return await query
                .Select(f => new Response(
                    f.Id.Value,
                    f.Code.Value,
                    f.Name,
                    f.Description,
                    f.IsActive,
                    f.CreatedAt,
                    f.UpdatedAt))
                .ToPagedResultAsync(
                    request.Page,
                    request.PageSize,
                    cancellationToken);
        }

        private static IQueryable<Domain.Faculty.Faculty> ApplySorting(IQueryable<Domain.Faculty.Faculty> query,
            FacultySortBy sortBy,
            SortDirection direction)
        {
            return (sortBy, direction) switch
            {
                (FacultySortBy.Code, SortDirection.Asc) =>
                    query.OrderBy(f => f.Code).ThenBy(f => f.Id),

                (FacultySortBy.Code, SortDirection.Desc) =>
                    query.OrderByDescending(f => f.Code).ThenBy(f => f.Id),

                (FacultySortBy.Name, SortDirection.Asc) =>
                    query.OrderBy(f => f.Name).ThenBy(f => f.Id),

                (FacultySortBy.Name, SortDirection.Desc) =>
                    query.OrderByDescending(f => f.Name).ThenBy(f => f.Id),

                (FacultySortBy.UpdatedAt, SortDirection.Asc) =>
                    query.OrderBy(f => f.UpdatedAt).ThenBy(f => f.Id),

                (FacultySortBy.UpdatedAt, SortDirection.Desc) =>
                    query.OrderByDescending(f => f.UpdatedAt).ThenBy(f => f.Id),

                (FacultySortBy.CreatedAt, SortDirection.Asc) =>
                    query.OrderBy(f => f.CreatedAt).ThenBy(f => f.Id),

                _ => query
                    .OrderByDescending(f => f.CreatedAt)
                    .ThenBy(f => f.Id)
            };
        }
    }

    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v{v:apiVersion}/faculties", async ([AsParameters] GetAllFacultiesRequest request, ISender sender, CancellationToken cancellationToken) =>
                    {
                        var query = request.Adapt<Query>();
                        var result = await sender.Send(query, cancellationToken);

                        return result.ToHttpResult();
                    })
                //.RequirePermission(PermissionCodes.Faculty.FacultiesRead)
                .Version(app, 1.0)
                .WithName("GetAllFaculties")
                .WithTags("Faculties");
        }
    }
}