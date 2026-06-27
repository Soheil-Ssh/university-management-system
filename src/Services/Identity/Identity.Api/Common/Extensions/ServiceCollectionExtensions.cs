using Carter;
using Identity.Api.Persistence.Contexts;
using Identity.Api.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedKernel.Abstractions;
using SharedKernel.Api;
using SharedKernel.Persistence;

namespace Identity.Api.Common.Extensions;

public static class ServiceCollectionExtensions
{
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

        // Add repositories and unit of work to the service collection
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();

        // TODO Apply migration
        // TODO Add seeds

        // Add Carter to the service collection
        services.AddCarter();

        // Add the shared kernel abstractions to the service collection
        services.AddSharedKernelAbstractions<Program>();

        // Add the shared kernel API services to the service collection
        services.AddSharedKernelApi();

        return services;
    }
}