using File.Api.Infrastructure.Persistence.Options;
using File.Api.Infrastructure.Persistence.Repositories;
using File.Api.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SharedKernel.Abstractions;
using SharedKernel.Api;
using SharedKernel.Observability.HealthCheck;
using SharedKernel.Persistence;

namespace File.Api.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFileServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Get the sql server connection string from the configuration
        var postgresConnectionString = configuration.GetConnectionString("PostgresDefaultConnection");

        // Add the shared kernel persistence services to the service collection
        services.AddSharedKernelPersistence();

        // Add the IdentityDbContext to the service collection
        services.AddDbContext<FileDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(postgresConnectionString);
        });

        // Add gRPC services to the service collection
        services.AddGrpc();

        // Add options to the service collection
        services.Configure<FileStorageOptions>(configuration.GetSection(FileStorageOptions.SectionName));

        // Add repositories to the service collection
        services.AddScoped<IFileRepository, FileRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Add services to the service collection
        services.AddScoped<IFileStorage, LocalFileStorage>();
        services.AddScoped<IFilePathGenerator, LocalFilePathGenerator>();

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
                check: () => HealthCheckResult.Healthy("File API is running."),
                tags: [HealthCheckTags.Live, HealthCheckTags.Ready, HealthCheckTags.Api])
            .AddNpgSql(
                connectionString: postgresConnectionString!,
                name: HealthCheckNames.DatabasePostgresSql,
                failureStatus: HealthStatus.Unhealthy,
                tags: [HealthCheckTags.Ready, HealthCheckTags.Database, HealthCheckTags.PostgresSql]);

        return services;
    }
}