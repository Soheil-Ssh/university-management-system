namespace Faculty.Api.Infrastructure.Persistence.Repositories;

public class ProfessorRepository(FacultyDbContext context) : IProfessorRepository
{
    public async Task<Professor?> GetByIdAsync(ProfessorId professorId, CancellationToken cancellationToken = default)
        => await context.Professors.FirstOrDefaultAsync(f => f.Id == professorId, cancellationToken);

    public async Task AddAsync(Professor professor, CancellationToken cancellationToken = default)
    {
        await context.Professors.AddAsync(professor, cancellationToken);
    }

    public async Task<bool> ExistsByNationalCodeAsync(NationalCode nationalCode, CancellationToken cancellationToken = default)
        => await context.Professors.AsNoTracking().AnyAsync(p => p.NationalCode == nationalCode, cancellationToken);

    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
        => await context.Professors.AsNoTracking().AnyAsync(p => p.Email == email, cancellationToken);

    public async Task<bool> ExistsByMobileNumberAsync(MobileNumber mobileNumber, CancellationToken cancellationToken = default)
        => await context.Professors.AsNoTracking().AnyAsync(p => p.MobileNumber == mobileNumber, cancellationToken);

    public async Task<int> GetNextCodeNumberAsync(CancellationToken cancellationToken = default)
    {
        var prefix = "UMS_FAC_PROF";

        var lastCode = await context.Professors
            .AsNoTracking()
            .OrderByDescending(u => u.Code)
            .Select(u => u.Code)
            .FirstOrDefaultAsync(cancellationToken);

        if (lastCode is null)
            return 1;

        var sequencePart = lastCode.Value[prefix.Length..];

        if (!int.TryParse(sequencePart, out var lastSequenceNumber))
            return 1;

        return lastSequenceNumber + 1;
    }
}