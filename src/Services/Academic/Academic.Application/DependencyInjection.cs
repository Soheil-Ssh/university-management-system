using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Academic.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // Add the shared kernel abstractions to the service collection
        services.AddSharedKernelAbstractions<AssemblyMarker>();

        return services;
    }
}

internal class AssemblyMarker;