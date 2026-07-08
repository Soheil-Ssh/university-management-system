using Serilog;
using SharedKernel.Observability;
using SharedKernel.Observability.Logging;

const string serviceName = "student-api";
ApplicationLogging.ConfigureBootstrapLogger(serviceName);

try
{
    Log.Information("Starting {ServiceName}", serviceName);

    var builder = WebApplication.CreateBuilder(args);
    builder.AddApplicationObservability(serviceName);
    builder.Services.AddStudentServices(builder.Configuration);

    var app = builder.Build();
    app.UseApplicationObservability();
    await app.UseStudentPipeline();
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