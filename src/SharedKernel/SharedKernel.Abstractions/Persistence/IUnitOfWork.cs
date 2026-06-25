namespace SharedKernel.Abstractions.Persistence;

public interface IUnitOfWork
{
    Task SaveAsync(CancellationToken cancellationToken = default);
}