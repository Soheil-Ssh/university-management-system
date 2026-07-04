using File.Api.Application.Protos;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedKernel.Abstractions;
using SharedKernel.Api;
using SharedKernel.Persistence;
using Student.Api.Infrastructure.FileServices;
using Student.Api.Infrastructure.Persistence.Repositories;
using Student.Api.Infrastructure.Security;

namespace Student.Api.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Get the sql server connection string from the configuration
        var postgresConnectionString = configuration.GetConnectionString("PostgresDefaultConnection") 
                                       ?? throw new InvalidOperationException("Postgres database connection is not configured.");

        // Add the shared kernel persistence services to the service collection
        services.AddSharedKernelPersistence();

        // Add the StudentDbContext to the service collection
        services.AddDbContext<StudentDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(postgresConnectionString);
        });

        // Get the file service gRPC URL from the configuration
        var fileServiceUrl = configuration["GrpcServices:FileServiceUrl"]
                             ?? throw new InvalidOperationException("File service gRPC URL is not configured.");

        Console.WriteLine($"========== File gRPC URL: {fileServiceUrl} ==========");

        // Add the gRPC client for the FileValidationService to the service collection
        services.AddGrpcClient<FileValidationService.FileValidationServiceClient>((serviceProvider, options) =>
        {
            options.Address = new Uri(fileServiceUrl);
        });

        // Add repositories and unit of work to the service collection
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAdmissionRequestRepository, AdmissionRequestRepository>();

        // Add services to the service collection
        services.AddScoped<IFileValidator, GrpcFileValidator>();
        services.AddSingleton<IRegistrationTokenGenerator, RegistrationTokenGenerator>();

        // Add the shared kernel abstractions to the service collection
        services.AddSharedKernelAbstractions<Program>();

        // Add the shared kernel API services to the service collection
        services.AddSharedKernelApi();

        // Add Carter to the service collection
        services.AddCarter();

        return services;
    }
}