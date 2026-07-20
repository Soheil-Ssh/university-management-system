namespace Academic.Application.Features.Majors.Commands.Create;

public sealed record CreateMajorCommand(Guid DepartmentId, string Name, string? Description) : ICommand<Result<Guid>>;