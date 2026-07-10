using Carter;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SharedKernel.Abstractions;
using SharedKernel.Api;
using SharedKernel.Identity;
using SharedKernel.Identity.Extensions;
using SharedKernel.Observability.HealthCheck;
using SharedKernel.Persistence;

namespace Notification.Api.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNotificationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Get the sql server connection string from the configuration
        var postgresConnectionString = configuration.GetConnectionString("PostgresDefaultConnection")
                                       ?? throw new InvalidOperationException("Postgres database connection is not configured.");

        // Add the shared kernel persistence services to the service collection
        services.AddSharedKernelPersistence();

        // Add the StudentDbContext to the service collection
        services.AddDbContext<NotificationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(postgresConnectionString);
        });

        // Add authentication to the service collection
        services.AddUmsJwtAuthentication(configuration);
        services.AddUmsAuthorization();

        // Add repositories and unit of work to the service collection
        //services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Add the shared kernel abstractions to the service collection
        services.AddSharedKernelAbstractions<Program>();

        // Add the shared kernel API services to the service collection
        services.AddSharedKernelApi();

        // Add Carter to the service collection
        services.AddCarter();

        // Add health checks to the service collection
        services.AddHealthChecks()
            .AddCheck(
                name: HealthCheckNames.Api,
                check: () => HealthCheckResult.Healthy("Student API is running."),
                tags: [HealthCheckTags.Live, HealthCheckTags.Ready, HealthCheckTags.Api])
            .AddNpgSql(
                connectionString: postgresConnectionString!,
                name: HealthCheckNames.DatabasePostgresSql,
                failureStatus: HealthStatus.Unhealthy,
                tags: [HealthCheckTags.Ready, HealthCheckTags.Database, HealthCheckTags.PostgresSql]);

        return services;
    }
}