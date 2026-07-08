using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            var observabilityOptions =
                builder.Configuration.GetSection(ObservabilityOptions.SectionName).Get<ObservabilityOptions>() ?? new ObservabilityOptions();

            var environmentName = builder.Environment.EnvironmentName;
            var applicationName = observabilityOptions.ApplicationName;

            loggerConfiguration
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("Application", applicationName)
                .Enrich.WithProperty("ServiceName", serviceName)
                .Enrich.WithProperty("Environment", environmentName)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
                .WriteTo.Console(outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] [{ServiceName}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}");

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
                var requestPath = httpContext.Request.Path;

                if (IsHealthCheckRequest(requestPath))
                {
                    if (statusCode >= StatusCodes.Status500InternalServerError)
                        return LogEventLevel.Warning;

                    if (statusCode >= StatusCodes.Status400BadRequest)
                        return LogEventLevel.Warning;

                    return LogEventLevel.Verbose;
                }

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

                if (httpContext.Request.Headers.TryGetValue(CorrelationIdConstants.HeaderName, out var correlationId))
                    diagnosticContext.Set(CorrelationIdConstants.LogPropertyName, correlationId.ToString());

                if (httpContext.User.Identity?.IsAuthenticated == true)
                    diagnosticContext.Set("UserId",
                        httpContext.User.FindFirst("sub")?.Value ?? httpContext.User.FindFirst("userId")?.Value ?? "unknown");
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

        loggerConfiguration
            .WriteTo
            .GrafanaLoki(lokiOptions.Uri,
                labels:
                [
                    new LokiLabel { Key = "app", Value = applicationName },
                    new LokiLabel { Key = "service", Value = serviceName },
                    new LokiLabel { Key = "env", Value = environmentName }
                ],
                restrictedToMinimumLevel: minimumLevel);
    }

    private static LogEventLevel ParseLogLevel(string? level)
    {
        if (Enum.TryParse<LogEventLevel>(level, ignoreCase: true, out var parsedLevel))
            return parsedLevel;

        return LogEventLevel.Information;
    }

    private static bool IsHealthCheckRequest(PathString path)
        => path.StartsWithSegments("/health") || path.StartsWithSegments("/health-checks-api");
}