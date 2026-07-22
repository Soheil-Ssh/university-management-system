using Academic.Application.Features.Majors.Queries.GetAll;
using Academic.Application.Features.Majors.Queries.GetById;

namespace Academic.Application.Queries;

public interface IMajorQueries
{
    Task<Result<PagedResult<GetAllMajorDto>>> GetAllAsync(GetAllMajorsQuery query, CancellationToken cancellationToken);
    Task<GetMajorByIdDto?> GetByIdAsync(MajorId id, CancellationToken cancellationToken);
}