namespace Academic.Application.Features.Majors.Queries.GetAll;

public sealed class GetAllMajorsQueryHandler(IMajorQueries majorQueries) : IQueryHandler<GetAllMajorsQuery, Result<PagedResult<GetAllMajorDto>>>
{
    public Task<Result<PagedResult<GetAllMajorDto>>> Handle(GetAllMajorsQuery request, CancellationToken cancellationToken)
        => majorQueries.GetAllAsync(request, cancellationToken);
}