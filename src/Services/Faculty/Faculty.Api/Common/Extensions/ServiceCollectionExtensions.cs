using Faculty.Api.Infrastructure.Messaging.Sagas;
using Faculty.Api.Infrastructure.Messaging.Sagas.Activities;
using Faculty.Api.Infrastructure.Messaging.Sagas.States;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SharedKernel.Abstractions;
using SharedKernel.Api;
using SharedKernel.Identity;
using SharedKernel.Identity.Extensions;
using SharedKernel.Messaging.MassTransit;
using SharedKernel.Messaging.MassTransit.Enums;
using SharedKernel.Observability.HealthCheck;
using SharedKernel.Persistence;

namespace Faculty.Api.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCentralOrganizationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Get the postgres connection string from the configuration
        var postgresConnectionString = configuration.GetConnectionString("PostgresDefaultConnection")
                                       ?? throw new InvalidOperationException("Postgres database connection is not configured.");

        // Add the shared kernel persistence services to the service collection
        services.AddSharedKernelPersistence();

        // Add the IdentityDbContext to the service collection
        services.AddDbContext<FacultyDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(postgresConnectionString);
        });

        // Add authentication to the service collection
        services.AddUmsJwtAuthentication(configuration);
        services.AddUmsAuthorization();

        // Add repositories and unit of work to the service collection
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IFacultyRepository, FacultyRepository>();
        services.AddScoped<IProfessorRepository, ProfessorRepository>();

        // Add the shared kernel abstractions to the service collection
        services.AddSharedKernelAbstractions<Program>();

        // Add the shared kernel API services to the service collection
        services.AddSharedKernelApi();

        // Add Carter to the service collection
        services.AddCarter();

        // Add MassTransit with EF Outbox messaging to the service collection
        services.AddApplicationMessagingWithEfOutbox<FacultyDbContext>(configuration, 
            MessagingOutboxProvider.Postgres,
            busConfiguration =>
            {
                busConfiguration
                    .AddSagaStateMachine<ProfessorIdentityProvisioningStateMachine,
                        ProfessorIdentityProvisioningState>()
                    .EntityFrameworkRepository(repository =>
                    {
                        repository.ExistingDbContext<FacultyDbContext>();
                        repository.UsePostgres();
                    });
            });

        // Add saga activities to the service collection
        services.AddScoped<MarkProfessorIdentityProvisioningSucceededActivity>();
        services.AddScoped<MarkProfessorIdentityProvisioningFailedActivity>();

        // Add health checks to the service collection
        services.AddHealthChecks()
            .AddCheck(
                name: HealthCheckNames.Api,
                check: () => HealthCheckResult.Healthy("Faculty API is running."),
                tags: [HealthCheckTags.Live, HealthCheckTags.Ready, HealthCheckTags.Api])
            .AddNpgSql(
                connectionString: postgresConnectionString!,
                name: HealthCheckNames.DatabasePostgresSql,
                failureStatus: HealthStatus.Unhealthy,
                tags: [HealthCheckTags.Ready, HealthCheckTags.Database, HealthCheckTags.PostgresSql]);

        return services;
    }
}