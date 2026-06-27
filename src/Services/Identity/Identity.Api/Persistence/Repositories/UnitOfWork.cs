namespace Identity.Api.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    public Task SaveAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}