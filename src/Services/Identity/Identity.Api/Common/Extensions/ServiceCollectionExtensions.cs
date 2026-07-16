using Duende.IdentityServer.Services;
using Identity.Api.Features.Users.v1.ProvisionEmployeeUser;
using Identity.Api.Features.Users.v1.ProvisionProfessorUser;
using Identity.Api.Infrastructure.IdentityServer;
using Identity.Api.Infrastructure.Persistence.Options;
using Identity.Api.Infrastructure.Persistence.Repositories;
using Identity.Api.Infrastructure.Persistence.Seed;
using Identity.Api.Infrastructure.Security;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SharedKernel.Abstractions;
using SharedKernel.Api;
using SharedKernel.Contracts.Integration.Events.Identity.User.v1;
using SharedKernel.Identity;
using SharedKernel.Identity.Extensions;
using SharedKernel.Messaging.MassTransit;
using SharedKernel.Messaging.MassTransit.Enums;
using SharedKernel.Messaging.MassTransit.Extensions;
using SharedKernel.Observability.HealthCheck;
using SharedKernel.Persistence;
using SharedKernel.Persistence.Database;
using System.Security.Cryptography.X509Certificates;
using Identity.Api.Features.Users.v1.DeactivateProfessorUser;

namespace Identity.Api.Common.Extensions;

public static class ServiceCollectionExtensions
{
    [Obsolete("Obsolete")]
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Get the sql server connection string from the configuration
        var sqlServerConnectionString = configuration.GetConnectionString("SqlServerDefaultConnection");

        // Add the shared kernel persistence services to the service collection
        services.AddSharedKernelPersistence();

        // Add the IdentityDbContext to the service collection
        services.AddDbContext<IdentityDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlServer(sqlServerConnectionString);
        });

        // Add options to the service collection
        services.Configure<SuperAdminOptions>(configuration.GetSection("SuperAdmin"));

        // Add authentication to the service collection
        services.AddUmsJwtAuthentication(configuration, useJwtBearerAsDefaultScheme: false);
        services.AddUmsAuthorization();

        // Add IdentityServer to the service collection
        var identityBuilder = services.AddIdentityServer(options =>
        {
            options.IssuerUri = configuration["Jwt:ValidIssuer"] ?? configuration["Jwt:Authority"];

            options.UserInteraction.LoginUrl = "/auth/login";
            options.UserInteraction.LogoutUrl = "/auth/logout";
            options.UserInteraction.ErrorUrl = "/auth/error";

            options.Authentication.CookieLifetime = TimeSpan.FromHours(8);
            options.Authentication.CookieSlidingExpiration = true;

            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseInformationEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseSuccessEvents = true;

            options.EmitStaticAudienceClaim = true;
        })
          .AddInMemoryIdentityResources(IdentityServerConfig.IdentityResources)
          .AddInMemoryApiScopes(IdentityServerConfig.ApiScopes)
          .AddInMemoryApiResources(IdentityServerConfig.ApiResources)
          .AddInMemoryClients(IdentityServerConfig.Clients);

        var signingPfxPath = configuration["IdentityServer:Signing:PfxPath"];
        var signingPfxPassword = configuration["IdentityServer:Signing:PfxPassword"];

        if (!string.IsNullOrWhiteSpace(signingPfxPath) && File.Exists(signingPfxPath))
        {
            var cert = new X509Certificate2(
                signingPfxPath,
                signingPfxPassword,
                X509KeyStorageFlags.MachineKeySet |
                X509KeyStorageFlags.EphemeralKeySet);

            identityBuilder.AddSigningCredential(cert);
        }
        else
        {
            identityBuilder.AddDeveloperSigningCredential(persistKey: true);
        }

        // Add the IdentityServer profile service to the service collection
        services.AddScoped<IProfileService, IdentityServerProfileService>();

        // Add gRPC services to the service collection
        services.AddGrpc();

        // Add repositories and unit of work to the service collection
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();

        // Add security services to the service collection
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

        // Add data seeders to the service collection
        services.AddScoped<IDataSeeder, PermissionSeeder>();
        services.AddScoped<IDataSeeder, RoleSeeder>();
        services.AddScoped<IDataSeeder, RolePermissionSeeder>();
        services.AddScoped<IDataSeeder, UserSeeder>();

        // Add the shared kernel abstractions to the service collection
        services.AddSharedKernelAbstractions<Program>();

        // Add the shared kernel API services to the service collection
        services.AddSharedKernelApi();

        // Add Carter to the service collection
        services.AddCarter();

        // Add Razor Pages to the service collection
        services.AddRazorPages();

        // Add health checks to the service collection
        services.AddHealthChecks()
            .AddCheck(
                name: HealthCheckNames.Api,
                check: () => HealthCheckResult.Healthy("Identity API is running."),
                tags: [HealthCheckTags.Live, HealthCheckTags.Ready, HealthCheckTags.Api])
            .AddSqlServer(
                connectionString: sqlServerConnectionString!,
                name: HealthCheckNames.DatabaseSqlServer,
                failureStatus: HealthStatus.Unhealthy,
                tags: [HealthCheckTags.Ready, HealthCheckTags.Database, HealthCheckTags.SqlServer]);

        // Add integration event handlers to the service collection
        services.AddScoped<
            IIntegrationEventHandler<CreateEmployeeIdentityUserRequestedIntegrationEvent>,
            ProvisionEmployeeUser.IntegrationEventHandler>();
        services.AddScoped<
            IIntegrationEventHandler<CreateProfessorIdentityUserRequestedIntegrationEvent>,
            ProvisionProfessorUser.IntegrationEventHandler>();
        services.AddScoped<
            IIntegrationEventHandler<DeactivateProfessorIdentityUserRequestedIntegrationEvent>,
            DeactivateProfessorUser.IntegrationEventHandler>();

        // Add Masstransit messaging to the service collection
        services.AddApplicationMessagingWithEfOutbox<IdentityDbContext>(configuration, MessagingOutboxProvider.SqlServer,
            busConfigurator =>
            {
                busConfigurator.AddIntegrationEventConsumer<CreateEmployeeIdentityUserRequestedIntegrationEvent>("identity-create-employee-user");
                busConfigurator.AddIntegrationEventConsumer<CreateProfessorIdentityUserRequestedIntegrationEvent>("identity-create-professor-user");
                busConfigurator.AddIntegrationEventConsumer<DeactivateProfessorIdentityUserRequestedIntegrationEvent>("identity-deactivate-professor-user");
            });

        return services;
    }
}