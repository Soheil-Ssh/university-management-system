using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedKernel.Abstractions;
using SharedKernel.Api;
using SharedKernel.Persistence;

namespace Student.Api.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Get the sql server connection string from the configuration
        var postgresConnectionString = configuration.GetConnectionString("PostgresDefaultConnection");

        // Add the shared kernel persistence services to the service collection
        services.AddSharedKernelPersistence();

        // Add the StudentDbContext to the service collection
        services.AddDbContext<StudentDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(postgresConnectionString);
        });

        // Add repositories and unit of work to the service collection
        //services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Add the shared kernel abstractions to the service collection
        services.AddSharedKernelAbstractions<Program>();

        // Add the shared kernel API services to the service collection
        services.AddSharedKernelApi();

        // Add Carter to the service collection
        services.AddCarter();

        return services;
    }
}