namespace Academic.Infrastructure.Persistence.Repositories;

public sealed class MajorRepository(AcademicDbContext context) : IMajorRepository
{
    public async Task<Major?> GetByIdAsync(MajorId id, CancellationToken cancellationToken = default)
        => await context.Majors.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task AddAsync(Major major, CancellationToken cancellationToken = default)
    {
        await context.Majors.AddAsync(major, cancellationToken);
    }

    public async Task<int> GetNextMajorCodeAsync(CancellationToken cancellationToken)
    {
        var prefix = "UMS_AC_MAJ_";

        var lastCode = await context.Majors
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