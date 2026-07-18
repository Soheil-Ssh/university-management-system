namespace Faculty.Api.Domain.DepartmentProfessorAssignment;

public interface IDepartmentProfessorAssignmentRepository
{
    Task<DepartmentProfessorAssignment?> GetByIdAsync(DepartmentProfessorAssignmentId id,
        CancellationToken cancellationToken = default);
    Task AddAsync(DepartmentProfessorAssignment assignment, CancellationToken cancellationToken = default);
    Task<bool> ExistsActiveAsync(DepartmentId departmentId, ProfessorId professorId, CancellationToken cancellationToken = default);
}