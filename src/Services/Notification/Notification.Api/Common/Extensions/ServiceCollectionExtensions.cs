using Carter;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Notification.Api.Features.Notifications.SendEmployeeAccountCreatedNotification;
using Notification.Api.Infrastructure.Persistence.Repositories;
using Notification.Api.Infrastructure.Providers;
using SharedKernel.Abstractions;
using SharedKernel.Api;
using SharedKernel.Contracts.Integration.Events.CentralOrganization.Employees.v1;
using SharedKernel.Identity;
using SharedKernel.Identity.Extensions;
using SharedKernel.Messaging.Abstractions;
using SharedKernel.Messaging.MassTransit;
using SharedKernel.Messaging.MassTransit.Enums;
using SharedKernel.Messaging.MassTransit.Extensions;
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
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        // Add notification providers to the service collection
        services.AddScoped<IEmailSender, LoggingEmailSender>();
        services.AddScoped<ISmsSender, LoggingSmsSender>();
        services.AddScoped<IPushSender, LoggingPushSender>();
        services.AddScoped<INotificationDispatcher, NotificationDispatcher>();

        // Add the shared kernel abstractions to the service collection
        services.AddSharedKernelAbstractions<Program>();

        // Add the shared kernel API services to the service collection
        services.AddSharedKernelApi();

        // Add Carter to the service collection
        services.AddCarter();

        // Add integration event handlers to the service collection
        services.AddScoped<IIntegrationEventHandler<EmployeeAccountCreatedIntegrationEvent>,
            SendEmployeeAccountCreatedNotification.IntegrationEventHandler>();

        // Add Masstransit messaging to the service collection
        services.AddApplicationMessagingWithEfOutbox<NotificationDbContext>(configuration,
            MessagingOutboxProvider.Postgres,
            busConfigurator =>
            {
                busConfigurator.AddIntegrationEventConsumer<EmployeeAccountCreatedIntegrationEvent>("employee-account-create");
            });

        // Add health checks to the service collection
        services.AddHealthChecks()
            .AddCheck(
                name: HealthCheckNames.Api,
                check: () => HealthCheckResult.Healthy("Notification API is running."),
                tags: [HealthCheckTags.Live, HealthCheckTags.Ready, HealthCheckTags.Api])
            .AddNpgSql(
                connectionString: postgresConnectionString!,
                name: HealthCheckNames.DatabasePostgresSql,
                failureStatus: HealthStatus.Unhealthy,
                tags: [HealthCheckTags.Ready, HealthCheckTags.Database, HealthCheckTags.PostgresSql]);

        return services;
    }
}