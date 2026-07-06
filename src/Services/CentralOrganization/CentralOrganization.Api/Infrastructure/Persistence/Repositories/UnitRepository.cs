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
}