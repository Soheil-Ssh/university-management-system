namespace Academic.Application.Features.Majors.Queries.GetById;

public sealed record GetMajorByIdQuery(Guid Id) : IQuery<Result<GetMajorByIdDto>>;