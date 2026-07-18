using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using SharedKernel.Observability.Metrics;
using SharedKernel.Observability.Options;
using SharedKernel.Observability.Tracing;

namespace SharedKernel.Observability.OpenTelemetry;


internal static class OpenTelemetryExtensions
{
    public static WebApplicationBuilder AddApplicationOpenTelemetry(this WebApplicationBuilder builder, string serviceName, string serviceVersion, ObservabilityOptions options)
    {
        if (!options.Metrics.Enabled && !options.Tracing.Enabled)
            return builder;

        var serviceInstanceId = Environment.GetEnvironmentVariable("HOSTNAME") ?? Environment.MachineName;

        var openTelemetry = builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(
                    serviceName: serviceName,
                    serviceNamespace: options.ApplicationName,
                    serviceVersion: serviceVersion,
                    serviceInstanceId: serviceInstanceId)
                .AddAttributes(new Dictionary<string, object>
                {
                    ["deployment.environment.name"] = builder.Environment.EnvironmentName
                }));

        if (options.Metrics.Enabled)
            openTelemetry.WithMetrics(metrics => metrics.ConfigureApplicationMetrics(options.Metrics));

        if (options.Tracing.Enabled)
            openTelemetry.WithTracing(tracing => tracing.ConfigureApplicationTracing(
                serviceName,
                options.Tracing,
                options.Metrics));

        return builder;
    }
}
