using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTelemetry.Metrics;
using SharedKernel.Observability.Options;

namespace SharedKernel.Observability.Metrics;

internal static class MetricsExtensions
{
    public static MeterProviderBuilder ConfigureApplicationMetrics(this MeterProviderBuilder metrics,
        MetricsOptions options)
        => metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddPrometheusExporter(exporter =>
            {
                exporter.ScrapeEndpointPath = options.EndpointPath;
                exporter.TargetInfoEnabled = true;
                exporter.ScopeInfoEnabled = true;
            });

    public static WebApplication MapApplicationMetrics(this WebApplication app)
    {
        var options = app.Services.GetRequiredService<IOptions<ObservabilityOptions>>().Value.Metrics;

        if (!options.Enabled)
            return app;

        app.MapPrometheusScrapingEndpoint()
            .DisableHttpMetrics()
            .AllowAnonymous();

        return app;
    }
}