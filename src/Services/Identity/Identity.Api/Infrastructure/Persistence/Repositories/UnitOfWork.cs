using Identity.Api.Infrastructure.Persistence.Contexts;

namespace Identity.Api.Infrastructure.Persistence.Repositories;

public class UnitOfWork(IdentityDbContext context) : IUnitOfWork
{
    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}