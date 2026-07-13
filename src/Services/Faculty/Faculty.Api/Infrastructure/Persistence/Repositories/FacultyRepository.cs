namespace Faculty.Api.Infrastructure.Persistence.Repositories;

public class FacultyRepository(FacultyDbContext context) : IFacultyRepository
{
    public async Task<Domain.Faculty.Faculty?> GetByIdAsync(FacultyId id, CancellationToken cancellationToken = default)
        => await context.Faculties.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

    public async Task AddAsync(Domain.Faculty.Faculty faculty, CancellationToken cancellationToken = default)
    {
        await context.Faculties.AddAsync(faculty, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(FacultyCode code, CancellationToken cancellationToken = default)
        => await context.Faculties.AsNoTracking().AnyAsync(f => f.Code == code, cancellationToken);

    public async Task<bool> ExistsByNameAsync(string name, FacultyId? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim().ToUpperInvariant();
        return await context.Faculties
            .AsNoTracking()
            .AnyAsync(f => f.Name.ToUpper() == normalizedName && (excludeId == null || f.Id != excludeId), cancellationToken);
    }

    public async Task<int> GetNextFacultyCodeAsync(CancellationToken cancellationToken)
    {
        var prefix = "UMS_FAC_FACULTY_";

        var lastCode = await context.Faculties
            .AsNoTracking()
            .OrderByDescending(f => f.Code)
            .Select(f => f.Code)
            .FirstOrDefaultAsync(cancellationToken);

        if (lastCode is null)
            return 1;

        var sequencePart = lastCode.Value[prefix.Length..];

        if (!int.TryParse(sequencePart, out var lastSequenceNumber))
            return 1;

        return lastSequenceNumber + 1;
    }

    public async Task<bool> IsDeanOfAnotherFacultyAsync(ProfessorId professorId, FacultyId excludedFacultyId, CancellationToken cancellationToken = default)
        => await context.Faculties
            .AsNoTracking()
            .AnyAsync(f => f.DeanProfessorId == professorId && f.Id != excludedFacultyId, cancellationToken);
}