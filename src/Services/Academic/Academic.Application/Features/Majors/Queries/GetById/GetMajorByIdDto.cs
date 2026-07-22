namespace Academic.Application.Features.Majors.Queries.GetById;

public sealed record GetMajorByIdDto(Guid Id, Guid DepartmentId, string Code, string Name, string? Description, bool IsActive, DateTime CreatedAt, DateTime UpdatedAt);