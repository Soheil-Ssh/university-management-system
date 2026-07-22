using Academic.Application.Features.Majors.Common;

namespace Academic.Application.Features.Majors.Queries.GetAll;

public sealed class GetAllMajorsQueryHandler(IMajorQueries majorQueries) : IQueryHandler<GetAllMajorsQuery, Result<PagedResult<MajorDto>>>
{
    public Task<Result<PagedResult<MajorDto>>> Handle(GetAllMajorsQuery request, CancellationToken cancellationToken)
        => majorQueries.GetAllAsync(request, cancellationToken);
}