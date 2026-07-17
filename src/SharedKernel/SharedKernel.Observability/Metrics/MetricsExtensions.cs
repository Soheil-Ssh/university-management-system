using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using SharedKernel.Observability.Options;

namespace SharedKernel.Observability.Metrics;

public static class MetricsExtensions
{
    public static WebApplicationBuilder AddApplicationMetrics(this WebApplicationBuilder builder, 
        string serviceName, 
        string serviceVersion = "1.0")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);

        var observabilityOptions = builder.Configuration.GetSection(ObservabilityOptions.SectionName).Get<ObservabilityOptions>() ?? new ObservabilityOptions();

        var metricsOptions = observabilityOptions.Metrics;

        if (!metricsOptions.Enabled)
            return builder;

        if (string.IsNullOrWhiteSpace(metricsOptions.EndpointPath) || !metricsOptions.EndpointPath.StartsWith('/'))
            throw new InvalidOperationException("Observability:Metrics:EndpointPath must start with '/'.");

        var serviceInstanceId = Environment.GetEnvironmentVariable("HOSTNAME") ?? Environment.MachineName;

        builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(
                    serviceName: serviceName,
                    serviceNamespace: observabilityOptions.ApplicationName,
                    serviceInstanceId: serviceInstanceId,
                    serviceVersion: serviceVersion)
                .AddAttributes(new Dictionary<string, object>
                {
                    ["deployment.environment.name"] = builder.Environment.EnvironmentName
                }))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddPrometheusExporter(options =>
                {
                    options.ScrapeEndpointPath = metricsOptions.EndpointPath;
                    options.TargetInfoEnabled = true;
                    options.ScopeInfoEnabled = true;
                }));

        return builder;
    }

    public static WebApplication MapApplicationMetrics(this WebApplication app)
    {
        app.MapPrometheusScrapingEndpoint().DisableHttpMetrics().AllowAnonymous();
        return app;
    }
}