using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SharedKernel.Persistence.Database;

public static class SeederExtensions
{
    public static async Task SeedDatabaseAsync<TContext>(this IServiceProvider services,
        CancellationToken cancellationToken = default)
        where TContext : DbContext
    {
        using var scope = services.CreateScope();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TContext>>();

        var context = scope.ServiceProvider.GetRequiredService<TContext>();

        await using var transaction =
            await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var seeders = scope.ServiceProvider
                .GetServices<IDataSeeder>()
                .OrderBy(x => x.Order);

            foreach (var seeder in seeders)
            {
                logger.LogInformation("Executing {Seeder}...", seeder.GetType().Name);
                await seeder.SeedAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);

            logger.LogInformation("Database seeding completed.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex, "Database seeding failed.");
            throw;
        }
    }
}