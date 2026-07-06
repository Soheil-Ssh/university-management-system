
namespace CentralOrganization.Api.Domain.Unit;

public interface IUnitRepository
{
    Task<Unit?> GetById(UnitId id, CancellationToken cancellationToken = default);
    Task AddAsync(Unit unit, CancellationToken cancellationToken = default);
    Task<bool> IsExistCodeAsync(UnitCode code, CancellationToken cancellationToken = default);
    Task<int> GetNextUnitCodeAsync(CancellationToken cancellationToken);
}