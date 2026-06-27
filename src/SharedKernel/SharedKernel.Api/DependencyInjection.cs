using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Api.ExceptionHandling;

namespace SharedKernel.Api;

// ReSharper disable once ConvertToExtensionBlock
public static class DependencyInjection
{
    public static IServiceCollection AddSharedKernelApi(this IServiceCollection services)
    {
        return services
            .AddSharedExceptionHandling()
            .AddSharedOpenApi()
            .AddSharedApiVersioning();
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static IServiceCollection AddSharedExceptionHandling(this IServiceCollection services)
    {
        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static IServiceCollection AddSharedOpenApi(this IServiceCollection services)
    {
        return services.AddOpenApi();
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static IServiceCollection AddSharedApiVersioning(this IServiceCollection services)
    {
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