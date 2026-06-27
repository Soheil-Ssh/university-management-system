using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SharedKernel.Persistence.Database;

public static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync<TContext>(this IServiceProvider services,
        CancellationToken cancellationToken = default)
        where TContext : DbContext
    {
        using var scope = services.CreateScope();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TContext>>();

        const int maxRetries = 30;

        for (var retry = 1; retry <= maxRetries; retry++)
        {
            try
            {
                logger.LogInformation("Applying migrations for {Context}...", typeof(TContext).Name);

                var context = scope.ServiceProvider.GetRequiredService<TContext>();

                await context.Database.MigrateAsync(cancellationToken);

                logger.LogInformation("Database migration completed successfully.");

                return;
            }
            catch (SqlException ex) when (retry < maxRetries)
            {
                logger.LogWarning(ex, "Database is not ready ({Retry}/{MaxRetries}). Retrying in 5 seconds...", retry, maxRetries);
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database migration failed.");
                throw;
            }
        }

        throw new InvalidOperationException("Database migration failed after maximum retry attempts.");
    }
}