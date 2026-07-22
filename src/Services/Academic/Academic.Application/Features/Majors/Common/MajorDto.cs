namespace Academic.Application.Features.Majors.Common;

public sealed record MajorDto(Guid Id, Guid DepartmentId, string Code, string Name, bool IsActive, DateTime CreatedAt, DateTime UpdatedAt);