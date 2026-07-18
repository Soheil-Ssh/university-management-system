using Serilog;

namespace SharedKernel.Observability.Logging;

public static class ApplicationLogging
{
    public static void ConfigureBootstrapLogger(string serviceName, string applicationName = "UMS")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceName);
        ArgumentException.ThrowIfNullOrWhiteSpace(applicationName);

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", applicationName)
            .Enrich.WithProperty("ServiceName", serviceName)
            .WriteTo.Console(outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level:u3}] [{ServiceName}] [{CorrelationId}] [{TraceId}:{SpanId}] " +
                "{Message:lj}{NewLine}{Exception}")
            .CreateBootstrapLogger();
    }
}