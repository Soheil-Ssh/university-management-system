using Academic.Application;
using Academic.Infrastructure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SharedKernel.Api;
using SharedKernel.Identity;
using SharedKernel.Identity.Extensions;
using SharedKernel.Observability.HealthCheck;

namespace Academic.Api.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAcademicServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Get the postgres connection string from the configuration
        var postgresConnectionString = configuration.GetConnectionString("PostgresDefaultConnection")
                                       ?? throw new InvalidOperationException("Postgres database connection is not configured.");

        // Add infrastructure services to the service collection 
        services.AddInfrastructure(configuration);

        // Add application services to the service collection 
        services.AddApplication(configuration);

        // Add authentication to the service collection
        services.AddUmsJwtAuthentication(configuration);
        services.AddUmsAuthorization();

        // Add the shared kernel API services to the service collection
        services.AddSharedKernelApi();

        // Add Carter to the service collection
        services.AddCarter();

        // Add health checks to the service collection
        services.AddHealthChecks()
            .AddCheck(
                name: HealthCheckNames.Api,
                check: () => HealthCheckResult.Healthy("Academic API is running."),
                tags: [HealthCheckTags.Live, HealthCheckTags.Ready, HealthCheckTags.Api])
            .AddNpgSql(
                connectionString: postgresConnectionString!,
                name: HealthCheckNames.DatabasePostgresSql,
                failureStatus: HealthStatus.Unhealthy,
                tags: [HealthCheckTags.Ready, HealthCheckTags.Database, HealthCheckTags.PostgresSql]);

        return services;
    }
}