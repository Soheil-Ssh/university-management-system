using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Api.ExceptionHandling;

namespace SharedKernel.Api;

public static class DependenciesContainer
{
    public static IServiceCollection AddSharedKernelApi(this IServiceCollection services, IConfiguration configuration)
    {
        // Add exception handlers
        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }
}