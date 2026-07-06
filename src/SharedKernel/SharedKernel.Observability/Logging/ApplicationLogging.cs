using Serilog;

namespace SharedKernel.Observability.Logging;

public static class ApplicationLogging
{
    public static void ConfigureBootstrapLogger(string serviceName)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "UMS")
            .Enrich.WithProperty("ServiceName", serviceName)
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{ServiceName}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}")
            .CreateBootstrapLogger();
    }
}