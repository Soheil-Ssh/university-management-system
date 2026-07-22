using Academic.Application.Features.Majors.Common;
using Academic.Application.Features.Majors.Queries.GetAll;

namespace Academic.Application.Queries;

public interface IMajorQueries
{
    Task<Result<PagedResult<MajorDto>>> GetAllAsync(GetAllMajorsQuery query, CancellationToken cancellationToken);
}