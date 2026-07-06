
namespace CentralOrganization.Api.Infrastructure.Persistence.Repositories;

public class UnitOfWork(CentralOrganizationDbContext context) : IUnitOfWork
{
    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}