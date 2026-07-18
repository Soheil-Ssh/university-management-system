using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Observability.HealthCheck;
using SharedKernel.Observability.Logging;
using SharedKernel.Observability.Metrics;
using SharedKernel.Observability.OpenTelemetry;
using SharedKernel.Observability.Options;

namespace SharedKernel.Observability;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddApplicationObservability(this WebApplicationBuilder builder, string serviceName, string serviceVersion = "1.0")
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceVersion);

        var section = builder.Configuration.GetSection(ObservabilityOptions.SectionName);
        var options = section.Get<ObservabilityOptions>() ?? new ObservabilityOptions();

        builder.Services
            .AddOptions<ObservabilityOptions>()
            .Bind(section)
            .Validate(value => !string.IsNullOrWhiteSpace(value.ApplicationName),
                "Observability:ApplicationName is required.")
            .Validate(value => !value.Loki.Enabled || Uri.TryCreate(value.Loki.Uri, UriKind.Absolute, out _),
                "Observability:Loki:Uri must be a valid absolute URI when Loki is enabled.")
            .Validate(value => !value.Metrics.Enabled || IsValidPath(value.Metrics.EndpointPath),
                "Observability:Metrics:EndpointPath must start with '/'.")
            .Validate(value => !value.Tracing.Enabled || Uri.TryCreate(value.Tracing.Endpoint, UriKind.Absolute, out _),
                "Observability:Tracing:Endpoint must be a valid absolute URI when tracing is enabled.")
            .Validate(value => value.Tracing.SamplingRatio is >= 0 and <= 1,
                "Observability:Tracing:SamplingRatio must be between 0 and 1.")
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            .Validate(value => value.Tracing.ExcludedPathPrefixes is not null &&
                               value.Tracing.ExcludedPathPrefixes.All(IsValidPath),
                "Every Observability:Tracing:ExcludedPathPrefixes value must start with '/'.")
            .ValidateOnStart();

        builder.AddApplicationSerilog(serviceName);
        builder.AddApplicationOpenTelemetry(serviceName, serviceVersion, options);

        return builder;
    }

    public static WebApplication UseApplicationObservability(this WebApplication app)
    {
        app.UseApplicationSerilogRequestLogging();
        return app;
    }

    public static WebApplication MapApplicationObservability(this WebApplication app)
    {
        app.MapApplicationHealthChecks();
        app.MapApplicationMetrics();
        return app;
    }

    private static bool IsValidPath(string? path)
        => !string.IsNullOrWhiteSpace(path) && path.StartsWith('/');
}