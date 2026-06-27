using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Api.ExceptionHandling;

namespace SharedKernel.Api;

public static class DependenciesContainer
{
    public static IServiceCollection AddSharedKernelApi(this IServiceCollection services)
    {
        // Add exception handlers
        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        // Add open API
        services.AddOpenApi();

        // Add API versioning
        services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("X-Api-Version"));
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }
}