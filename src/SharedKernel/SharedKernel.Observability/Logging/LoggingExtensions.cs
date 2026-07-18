using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;
using SharedKernel.Observability.Correlation;
using SharedKernel.Observability.Options;

namespace SharedKernel.Observability.Logging;

public static class LoggingExtensions
{
    public static WebApplicationBuilder AddApplicationSerilog(this WebApplicationBuilder builder, string serviceName)
    {
        builder.Services.Configure<ObservabilityOptions>(
            builder.Configuration.GetSection(ObservabilityOptions.SectionName));

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddSerilog((services, loggerConfiguration) =>
        {
            var observabilityOptions = services.GetRequiredService<IOptions<ObservabilityOptions>>().Value;
            var environmentName = builder.Environment.EnvironmentName;
            var applicationName = observabilityOptions.ApplicationName;

            loggerConfiguration
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.With<ActivityEnricher>()
                .Enrich.WithProperty("Application", applicationName)
                .Enrich.WithProperty("ServiceName", serviceName)
                .Enrich.WithProperty("Environment", environmentName)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
                .WriteTo.Console(outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] [{ServiceName}] [{CorrelationId}] [{TraceId}:{SpanId}] " +
                    "{Message:lj}{NewLine}{Exception}");

            AddLokiSinkIfEnabled(loggerConfiguration,
                observabilityOptions,
                serviceName,
                environmentName,
                applicationName);
        });

        return builder;
    }

    public static WebApplication UseApplicationSerilogRequestLogging(this WebApplication app)
    {
        var observabilityOptions = app.Services.GetRequiredService<IOptions<ObservabilityOptions>>().Value;
        var metricsPath = observabilityOptions.Metrics.Enabled
            ? new PathString(observabilityOptions.Metrics.EndpointPath)
            : PathString.Empty;

        app.UseMiddleware<CorrelationIdMiddleware>();

        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate =
                "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

            options.GetLevel = (httpContext, elapsed, exception) =>
            {
                if (exception is not null)
                    return LogEventLevel.Error;

                var statusCode = httpContext.Response.StatusCode;

                if (IsObservabilityRequest(httpContext.Request.Path, metricsPath))
                    return statusCode >= StatusCodes.Status400BadRequest
                        ? LogEventLevel.Warning
                        : LogEventLevel.Verbose;

                if (httpContext.Response.StatusCode >= StatusCodes.Status500InternalServerError)
                    return LogEventLevel.Error;

                if (statusCode >= StatusCodes.Status400BadRequest)
                    return LogEventLevel.Warning;

                if (elapsed >= 1000)
                    return LogEventLevel.Warning;

                return LogEventLevel.Information;
            };

            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress?.ToString());
                diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
                diagnosticContext.Set("RequestPath", httpContext.Request.Path.Value);
                diagnosticContext.Set("StatusCode", httpContext.Response.StatusCode);

                if (httpContext.Items.TryGetValue(CorrelationIdConstants.HttpContextItemName, out var correlationId))
                    diagnosticContext.Set(CorrelationIdConstants.LogPropertyName, correlationId?.ToString());

                if (httpContext.User.Identity?.IsAuthenticated == true)
                {
                    diagnosticContext.Set("UserId",
                        httpContext.User.FindFirst("sub")?.Value ??
                        httpContext.User.FindFirst("userId")?.Value ?? "unknown");
                }
            };
        });

        return app;
    }

    private static void AddLokiSinkIfEnabled(LoggerConfiguration loggerConfiguration,
        ObservabilityOptions observabilityOptions,
        string serviceName,
        string environmentName,
        string applicationName)
    {
        var lokiOptions = observabilityOptions.Loki;

        if (!lokiOptions.Enabled)
            return;

        if (string.IsNullOrWhiteSpace(lokiOptions.Uri))
            return;

        var minimumLevel = ParseLogLevel(lokiOptions.MinimumLevel);

        loggerConfiguration.WriteTo.GrafanaLoki(lokiOptions.Uri,
                labels:
                [
                    new LokiLabel { Key = "app", Value = applicationName },
                    new LokiLabel { Key = "service", Value = serviceName },
                    new LokiLabel { Key = "env", Value = environmentName }
                ],
                restrictedToMinimumLevel: ParseLogLevel(lokiOptions.MinimumLevel));
    }

    private static LogEventLevel ParseLogLevel(string? level)
        => Enum.TryParse<LogEventLevel>(level, ignoreCase: true, out var parsedLevel) ? parsedLevel : LogEventLevel.Information;

    private static bool IsObservabilityRequest(PathString path, PathString metricsPath)
        => path.StartsWithSegments("/health") ||
           path.StartsWithSegments("/health-checks-api") ||
           (metricsPath.HasValue && path.StartsWithSegments(metricsPath));
}