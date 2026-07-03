namespace Student.Api.Infrastructure.Persistence.Repositories;

public class UnitOfWork(StudentDbContext context) : IUnitOfWork
{
    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}