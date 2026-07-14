namespace Faculty.Api.Domain.Department;

public interface IDepartmentRepository
{
    Task<Department?> GetByIdAsync(DepartmentId id, CancellationToken cancellationToken = default);
    Task AddAsync(Department department, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(FacultyId facultyId, string name, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(FacultyId facultyId, string name, DepartmentId departmentId, CancellationToken cancellationToken = default);
    Task<int> GetNextDepartmentCodeAsync(CancellationToken cancellationToken);
}