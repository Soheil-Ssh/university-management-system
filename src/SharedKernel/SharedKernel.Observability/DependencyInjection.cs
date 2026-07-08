using Microsoft.AspNetCore.Builder;
using SharedKernel.Observability.HealthCheck;
using SharedKernel.Observability.Logging;

namespace SharedKernel.Observability;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddApplicationObservability(this WebApplicationBuilder builder, string serviceName)
    {
        builder.AddApplicationSerilog(serviceName);
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
        return app;
    }
}