namespace Faculty.Api.Infrastructure.Persistence.Repositories;

public sealed class DepartmentRepository(FacultyDbContext context) : IDepartmentRepository
{
    public async Task<Department?> GetByIdAsync(DepartmentId id, CancellationToken cancellationToken = default)
        => await context.Departments.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public async Task<Department?> GetByHeadProfessorIdAsync(ProfessorId professorId, CancellationToken cancellationToken = default)
        => await context.Departments.FirstOrDefaultAsync(d => d.HeadProfessorId == professorId, cancellationToken);

    public async Task AddAsync(Department department, CancellationToken cancellationToken = default)
    {
        await context.Departments.AddAsync(department, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(FacultyId facultyId, string name, CancellationToken cancellationToken = default)
        => await context.Departments.AnyAsync(d => d.FacultyId == facultyId && d.Name == name, cancellationToken);

    public async Task<bool> ExistsByNameAsync(FacultyId facultyId, string name, DepartmentId departmentId, CancellationToken cancellationToken = default)
        => await context.Departments.AnyAsync(d => d.Id != departmentId && d.FacultyId == facultyId && d.Name == name, cancellationToken);

    public async Task<int> GetNextDepartmentCodeAsync(CancellationToken cancellationToken)
    {
        var prefix = "UMS_FAC_DEP_";

        var lastCode = await context.Departments
            .AsNoTracking()
            .OrderByDescending(d => d.Code)
            .Select(d => d.Code)
            .FirstOrDefaultAsync(cancellationToken);

        if (lastCode is null)
            return 1;

        var sequencePart = lastCode.Value[prefix.Length..];

        if (!int.TryParse(sequencePart, out var lastSequenceNumber))
            return 1;

        return lastSequenceNumber + 1;
    }

    public Task<bool> IsHeadOfAnotherDepartmentAsync(ProfessorId professorId, DepartmentId excludedDepartmentId, CancellationToken cancellationToken = default)
        => context.Departments
            .AsNoTracking()
            .AnyAsync(r => r.HeadProfessorId == professorId && r.Id != excludedDepartmentId, cancellationToken);
}