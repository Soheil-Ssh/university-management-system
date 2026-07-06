using CentralOrganization.Api.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedKernel.Abstractions;
using SharedKernel.Api;
using SharedKernel.Persistence;

namespace CentralOrganization.Api.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCentralOrganizationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Get the postgres connection string from the configuration
        var sqlServerConnectionString = configuration.GetConnectionString("PostgresDefaultConnection");

        // Add the shared kernel persistence services to the service collection
        services.AddSharedKernelPersistence();

        // Add the IdentityDbContext to the service collection
        services.AddDbContext<CentralOrganizationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(sqlServerConnectionString);
        });

        // Add repositories and unit of work to the service collection

        // Add the shared kernel abstractions to the service collection
        services.AddSharedKernelAbstractions<Program>();

        // Add the shared kernel API services to the service collection
        services.AddSharedKernelApi();

        // Add Carter to the service collection
        services.AddCarter();

        return services;
    }
}