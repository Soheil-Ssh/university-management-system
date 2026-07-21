namespace Academic.Application.Abstractions.Services;

public interface IDepartmentValidationService
{
    Task<Result> ValidateForMajorAsync(Guid departmentId, CancellationToken cancellationToken);
}