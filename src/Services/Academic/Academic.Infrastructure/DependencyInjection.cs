using Academic.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Abstractions.Persistence;
using SharedKernel.Persistence;

namespace Academic.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Get the postgres connection string from the configuration
        var postgresConnectionString = configuration.GetConnectionString("PostgresDefaultConnection")
                                       ?? throw new InvalidOperationException("Postgres database connection is not configured.");

        // Add the shared kernel persistence services to the service collection
        services.AddSharedKernelPersistence();

        // Add the IdentityDbContext to the service collection
        services.AddDbContext<AcademicDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(postgresConnectionString);
        });

        // Add repositories and unit of work to the service collection
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IMajorRepository, MajorRepository>();
        services.AddScoped<ICurriculumRepository, CurriculumRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();

        return services;
    }
}