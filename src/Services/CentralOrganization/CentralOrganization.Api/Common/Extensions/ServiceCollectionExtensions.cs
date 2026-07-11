using CentralOrganization.Api.Application.Abstractions;
using CentralOrganization.Api.Infrastructure.Grpc;
using CentralOrganization.Api.Infrastructure.Messaging.Sagas;
using CentralOrganization.Api.Infrastructure.Messaging.Sagas.Activities;
using CentralOrganization.Api.Infrastructure.Messaging.Sagas.States;
using CentralOrganization.Api.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SharedKernel.Abstractions;
using SharedKernel.Api;
using SharedKernel.Contracts.Grpc.File.v1;
using SharedKernel.Identity;
using SharedKernel.Identity.Extensions;
using SharedKernel.Messaging.MassTransit;
using SharedKernel.Messaging.MassTransit.Enums;
using SharedKernel.Observability.HealthCheck;
using SharedKernel.Persistence;

namespace CentralOrganization.Api.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCentralOrganizationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Get the postgres connection string from the configuration
        var postgresConnectionString = configuration.GetConnectionString("PostgresDefaultConnection")
                                       ?? throw new InvalidOperationException("Postgres database connection is not configured.");

        // Add the shared kernel persistence services to the service collection
        services.AddSharedKernelPersistence();

        // Add the IdentityDbContext to the service collection
        services.AddDbContext<CentralOrganizationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(postgresConnectionString);
        });

        // Add authentication to the service collection
        services.AddUmsJwtAuthentication(configuration);
        services.AddUmsAuthorization();

        // Get the file service gRPC URL from the configuration
        var fileServiceUrl = configuration["GrpcServices:FileServiceUrl"]
                             ?? throw new InvalidOperationException("File service gRPC URL is not configured.");

        // Add the gRPC client for the FileValidationService to the service collection
        services.AddGrpcClient<FileValidationService.FileValidationServiceClient>((serviceProvider, options) =>
        {
            options.Address = new Uri(fileServiceUrl);
        });

        // Add repositories and unit of work to the service collection
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUnitRepository, UnitRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        // Add services to the service collection
        services.AddScoped<IFileValidatorClient, FileValidatorGrpcClient>();

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
                check: () => HealthCheckResult.Healthy("Central Organization API is running."),
                tags: [HealthCheckTags.Live, HealthCheckTags.Ready, HealthCheckTags.Api])
            .AddNpgSql(
                connectionString: postgresConnectionString!,
                name: HealthCheckNames.DatabasePostgresSql,
                failureStatus: HealthStatus.Unhealthy,
                tags: [HealthCheckTags.Ready, HealthCheckTags.Database, HealthCheckTags.PostgresSql]);

        // Add Masstransit messaging to the service collection
        services.AddApplicationMessagingWithEfOutbox<CentralOrganizationDbContext>(configuration,
            MessagingOutboxProvider.Postgres,
            busConfiguration =>
            {
                busConfiguration
                    .AddSagaStateMachine<EmployeeIdentityProvisioningStateMachine, EmployeeIdentityProvisioningState>()
                    .EntityFrameworkRepository(repository =>
                    {
                        repository.ExistingDbContext<CentralOrganizationDbContext>();
                        repository.UsePostgres();
                    });
            });

        // Add saga activities to the service collection
        services.AddScoped<MarkEmployeeIdentityProvisioningSucceededActivity>();
        services.AddScoped<MarkEmployeeIdentityProvisioningFailedActivity>();

        return services;
    }
}