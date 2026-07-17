using Microsoft.AspNetCore.Builder;
using SharedKernel.Observability.HealthCheck;
using SharedKernel.Observability.Logging;
using SharedKernel.Observability.Metrics;

namespace SharedKernel.Observability;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddApplicationObservability(this WebApplicationBuilder builder,
        string serviceName,
        string serviceVersion = "1.0")
    {
        builder.AddApplicationSerilog(serviceName);
        builder.AddApplicationMetrics(serviceName, serviceVersion);
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
}