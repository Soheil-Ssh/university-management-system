namespace Academic.Application.Features.Majors.Commands.Update;

public sealed record UpdateMajorCommand(Guid MajorId, Guid DepartmentId, string Name, string? Description) : ICommand<Result>;