using CentralOrganization.Api.Infrastructure.Grpc;
using CentralOrganization.Api.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedKernel.Abstractions;
using SharedKernel.Api;
using SharedKernel.Contracts.Grpc.Identity.v1;
using SharedKernel.Identity;
using SharedKernel.Identity.Extensions;
using SharedKernel.Persistence;

namespace CentralOrganization.Api.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCentralOrganizationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Get the postgres connection string from the configuration
        var sqlServerConnectionString = configuration.GetConnectionString("PostgresDefaultConnection")
                                        ?? throw new InvalidOperationException("Postgres database connection is not configured.");

        // Add the shared kernel persistence services to the service collection
        services.AddSharedKernelPersistence();

        // Add the IdentityDbContext to the service collection
        services.AddDbContext<CentralOrganizationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(sqlServerConnectionString);
        });

        // Add authentication to the service collection
        services.AddUmsJwtAuthentication(configuration);
        services.AddUmsAuthorization();

        // Get the identity service gRPC URL from the configuration
        var identityServiceUrl = configuration["GrpcServices:IdentityServiceUrl"]
                             ?? throw new InvalidOperationException("File service gRPC URL is not configured.");

        // Add the gRPC client for the FileValidationService to the service collection
        services.AddGrpcClient<IdentityUserService.IdentityUserServiceClient>((serviceProvider, options) =>
        {
            options.Address = new Uri(identityServiceUrl);
        });

        // Add repositories and unit of work to the service collection
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUnitRepository, UnitRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IIdentityUserClient, IdentityUserGrpcClient>();

        // Add the shared kernel abstractions to the service collection
        services.AddSharedKernelAbstractions<Program>();

        // Add the shared kernel API services to the service collection
        services.AddSharedKernelApi();

        // Add Carter to the service collection
        services.AddCarter();

        return services;
    }
}