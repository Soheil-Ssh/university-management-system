using Academic.Application.Features.Majors.Queries.GetAll;
using Academic.Application.Features.Majors.Queries.GetById;
using Academic.Application.Queries;

namespace Academic.Infrastructure.Queries;

public class MajorQueries(AcademicDbContext context) : IMajorQueries
{
    public async Task<Result<PagedResult<GetAllMajorDto>>> GetAllAsync(GetAllMajorsQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Major> query = context.Majors.AsNoTracking();

        if (request.DepartmentId.HasValue)
            query = query.Where(x => x.DepartmentId == new DepartmentId(request.DepartmentId.Value));

        if (!string.IsNullOrWhiteSpace(request.Code))
        {
            var codeResult = MajorCode.FromString(request.Code.Trim());

            if (codeResult.IsFailure)
                return codeResult.Error;

            query = query.Where(x => x.Code == codeResult.Data);
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var name = request.Name.Trim();
            query = query.Where(x => EF.Functions.ILike(x.Name, $"%{name}%"));
        }

        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        if (request.FromCreatedAt.HasValue)
            query = query.Where(x => x.CreatedAt >= request.FromCreatedAt.Value);

        if (request.ToCreatedAt.HasValue)
            query = query.Where(x => x.CreatedAt <= request.ToCreatedAt.Value);

        if (request.FromUpdatedAt.HasValue)
            query = query.Where(x => x.UpdatedAt >= request.FromUpdatedAt.Value);

        if (request.ToUpdatedAt.HasValue)
            query = query.Where(x => x.UpdatedAt <= request.ToUpdatedAt.Value);

        query = ApplySorting(query, request.SortBy, request.SortDirection);

        var responseQuery = query.Select(x => new GetAllMajorDto(
            x.Id.Value,
            x.DepartmentId.Value,
            x.Code.Value,
            x.Name,
            x.IsActive,
            x.CreatedAt,
            x.UpdatedAt));

        return await responseQuery.ToPagedResultAsync(request.Page, request.PageSize, cancellationToken);
    }

    public async Task<GetMajorByIdDto?> GetByIdAsync(MajorId id, CancellationToken cancellationToken)
        => await context.Majors.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new GetMajorByIdDto(
                x.Id.Value,
                x.DepartmentId.Value,
                x.Code.Value,
                x.Name,
                x.Description,
                x.IsActive,
                x.CreatedAt,
                x.UpdatedAt))
            .FirstOrDefaultAsync(cancellationToken);

    private static IQueryable<Major> ApplySorting(IQueryable<Major> query, MajorSortBy sortBy, SortDirection direction)
        => (sortBy, direction) switch
        {
            (MajorSortBy.Code, SortDirection.Asc) =>
                query.OrderBy(x => x.Code).ThenBy(x => x.Id),

            (MajorSortBy.Code, SortDirection.Desc) =>
                query.OrderByDescending(x => x.Code).ThenBy(x => x.Id),

            (MajorSortBy.Name, SortDirection.Asc) =>
                query.OrderBy(x => x.Name).ThenBy(x => x.Id),

            (MajorSortBy.Name, SortDirection.Desc) =>
                query.OrderByDescending(x => x.Name).ThenBy(x => x.Id),

            (MajorSortBy.UpdatedAt, SortDirection.Asc) =>
                query.OrderBy(x => x.UpdatedAt).ThenBy(x => x.Id),

            (MajorSortBy.UpdatedAt, SortDirection.Desc) =>
                query.OrderByDescending(x => x.UpdatedAt).ThenBy(x => x.Id),

            (MajorSortBy.CreatedAt, SortDirection.Asc) =>
                query.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id),

            _ => query.OrderByDescending(x => x.CreatedAt).ThenBy(x => x.Id)
        };
}