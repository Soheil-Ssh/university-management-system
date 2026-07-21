using Academic.Application.Abstractions.Services;
using Academic.Infrastructure.Grpc;
using Academic.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Abstractions.Persistence;
using SharedKernel.Contracts.Grpc.Faculty.v1;
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

        // Get the file service gRPC URL from the configuration
        var fileServiceUrl = configuration["GrpcServices:FacultyServiceUrl"]
                             ?? throw new InvalidOperationException("Faculty service gRPC URL is not configured.");

        // Add the gRPC client for the FileValidationService to the service collection
        services.AddGrpcClient<DepartmentValidationService.DepartmentValidationServiceClient>((serviceProvider, options) =>
        {
            options.Address = new Uri(fileServiceUrl);
        });

        // Add services to the service collection
        services.AddScoped<IDepartmentValidationService, DepartmentValidationGrpcClient>();

        return services;
    }
}