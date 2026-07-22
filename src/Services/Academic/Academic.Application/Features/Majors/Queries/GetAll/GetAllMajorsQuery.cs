using Academic.Application.Features.Majors.Common;

namespace Academic.Application.Features.Majors.Queries.GetAll;

public enum MajorSortBy
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

public sealed record GetAllMajorsQuery(
    Guid? DepartmentId,
    string? Code,
    string? Name,
    bool? IsActive,
    DateTime? FromCreatedAt,
    DateTime? ToCreatedAt,
    DateTime? FromUpdatedAt,
    DateTime? ToUpdatedAt,
    MajorSortBy SortBy = MajorSortBy.CreatedAt,
    SortDirection SortDirection = SortDirection.Desc,
    int Page = 1,
    int PageSize = 20) : IQuery<Result<PagedResult<MajorDto>>>;