namespace Academic.Domain.Major;

public interface IMajorRepository
{
    Task<Major?> GetByIdAsync(MajorId id, CancellationToken cancellationToken = default);
    Task AddAsync(Major major, CancellationToken cancellationToken = default);
    Task<bool> ExistsByDepartmentIdAsync(DepartmentId departmentId, CancellationToken cancellationToken = default);
    Task<int> GetNextMajorCodeAsync(CancellationToken cancellationToken);
}