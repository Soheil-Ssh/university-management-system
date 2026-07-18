namespace Faculty.Api.Infrastructure.Persistence.Repositories;

public sealed class DepartmentProfessorAssignmentRepository(FacultyDbContext context) : IDepartmentProfessorAssignmentRepository
{
    public async Task<DepartmentProfessorAssignment?> GetByIdAsync(DepartmentProfessorAssignmentId id, CancellationToken cancellationToken = default)
        => await context.DepartmentProfessorAssignments.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<DepartmentProfessorAssignment?> GetActiveAsync(DepartmentId departmentId,
        ProfessorId professorId,
        CancellationToken cancellationToken = default)
        => await context.DepartmentProfessorAssignments
            .FirstOrDefaultAsync(a => a.DepartmentId == departmentId && a.ProfessorId == professorId && a.UnassignedAt == null, cancellationToken);

    public async Task AddAsync(DepartmentProfessorAssignment assignment, CancellationToken cancellationToken = default)
    {
        await context.DepartmentProfessorAssignments.AddAsync(assignment, cancellationToken);
    }

    public async Task<bool> ExistsActiveAsync(DepartmentId departmentId, ProfessorId professorId, CancellationToken cancellationToken = default)
        => await context.DepartmentProfessorAssignments
            .AnyAsync(a => a.DepartmentId == departmentId && a.ProfessorId == professorId, cancellationToken);
}