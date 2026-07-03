namespace File.Api.Infrastructure.Persistence.Repositories;

public class UnitOfWork(FileDbContext context) : IUnitOfWork
{
    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}