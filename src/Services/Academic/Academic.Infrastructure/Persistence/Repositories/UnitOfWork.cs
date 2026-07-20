using SharedKernel.Abstractions.Persistence;

namespace Academic.Infrastructure.Persistence.Repositories;

public sealed class UnitOfWork(AcademicDbContext context) : IUnitOfWork
{
    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}