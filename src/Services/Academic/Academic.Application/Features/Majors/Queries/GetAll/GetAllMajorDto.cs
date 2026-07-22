namespace Academic.Application.Features.Majors.Queries.GetAll;

public sealed record GetAllMajorDto(Guid Id, Guid DepartmentId, string Code, string Name, bool IsActive, DateTime CreatedAt, DateTime UpdatedAt);