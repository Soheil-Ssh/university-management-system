using Identity.Api.Infrastructure.Persistence.Contexts;
using Identity.Api.Infrastructure.Persistence.Repositories;
using Identity.Api.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedKernel.Abstractions;
using SharedKernel.Api;
using SharedKernel.Persistence;
using SharedKernel.Persistence.Database;

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

        // Add data seeders to the service collection
        services.AddScoped<IDataSeeder, PermissionSeeder>();
        services.AddScoped<IDataSeeder, RoleSeeder>();
        services.AddScoped<IDataSeeder, RolePermissionSeeder>();

        // Add the shared kernel abstractions to the service collection
        services.AddSharedKernelAbstractions<Program>();

        // Add the shared kernel API services to the service collection
        services.AddSharedKernelApi();

        // Add Carter to the service collection
        services.AddCarter();

        return services;
    }
}