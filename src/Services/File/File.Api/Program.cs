using Serilog;
using SharedKernel.Observability;
using SharedKernel.Observability.Logging;

const string serviceName = "file-api";
ApplicationLogging.ConfigureBootstrapLogger(serviceName);

try
{
    Log.Information("Starting {ServiceName}", serviceName);

    var builder = WebApplication.CreateBuilder(args);
    builder.AddApplicationObservability(serviceName);
    builder.Services.AddFileServices(builder.Configuration);

    var app = builder.Build();
    app.UseApplicationObservability();
    await app.UseFilePipeline();
    app.MapApplicationObservability();

    app.Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "{ServiceName} terminated unexpectedly", serviceName);
}
finally
{
    await Log.CloseAndFlushAsync();
}