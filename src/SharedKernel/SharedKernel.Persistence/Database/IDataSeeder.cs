namespace SharedKernel.Persistence.Database;

public interface IDataSeeder
{
    int Order { get; }
    Task SeedAsync(CancellationToken cancellationToken = default);
}