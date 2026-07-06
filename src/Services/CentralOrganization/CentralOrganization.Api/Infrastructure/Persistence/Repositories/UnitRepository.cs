using Unit = CentralOrganization.Api.Domain.Unit.Unit;

namespace CentralOrganization.Api.Infrastructure.Persistence.Repositories;

public class UnitRepository(CentralOrganizationDbContext context) : IUnitRepository
{
    public async Task<Unit?> GetById(UnitId id, CancellationToken cancellationToken = default)
        => await context.Units.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task AddAsync(Unit unit, CancellationToken cancellationToken = default)
    {
        await context.Units.AddAsync(unit, cancellationToken);
    }

    public async Task<bool> IsExistCodeAsync(UnitCode code, CancellationToken cancellationToken = default)
        => await context.Units.AnyAsync(u => u.Code == code, cancellationToken);

    public async Task<int> GetNextUnitCodeAsync(CancellationToken cancellationToken)
    {
        var prefix = "UMS_CO_UNIT_";

        var lastCode = await context.Units
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