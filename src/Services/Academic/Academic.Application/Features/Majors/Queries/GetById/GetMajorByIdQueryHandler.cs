namespace Academic.Application.Features.Majors.Queries.GetById;

public sealed class GetMajorByIdQueryHandler(IMajorQueries majorQueries) : IQueryHandler<GetMajorByIdQuery, Result<GetMajorByIdDto>>
{
    public async Task<Result<GetMajorByIdDto>> Handle(GetMajorByIdQuery request, CancellationToken cancellationToken)
    {
        var major = await majorQueries.GetByIdAsync(new MajorId(request.Id), cancellationToken);
        if (major is null)
            return MajorErrors.NotFound;

        return major;
    }
}