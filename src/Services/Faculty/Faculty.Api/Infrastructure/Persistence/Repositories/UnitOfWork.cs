namespace Faculty.Api.Infrastructure.Persistence.Repositories;

public class UnitOfWork(FacultyDbContext context) : IUnitOfWork
{
    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}