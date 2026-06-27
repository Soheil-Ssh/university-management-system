using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Persistence.Interceptors;

namespace SharedKernel.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedKernelPersistence(this IServiceCollection services)
    {
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DomainEventsInterceptor>();

        return services;
    }
}