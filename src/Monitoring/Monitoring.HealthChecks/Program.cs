using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;
using Serilog;
using SharedKernel.Observability;
using SharedKernel.Observability.HealthCheck;
using SharedKernel.Observability.Logging;

const string serviceName = "monitoring-healthchecks";
ApplicationLogging.ConfigureBootstrapLogger(serviceName);

try
{
    Log.Information("Starting {ServiceName}", serviceName);

    var builder = WebApplication.CreateBuilder(args);
    builder.AddApplicationObservability(serviceName);

    var rabbitMqConnectionString = builder.Configuration.GetConnectionString("RabbitMq")
                               ?? throw new InvalidOperationException("ConnectionStrings:RabbitMq is not configured.");

    builder.Services.AddSingleton<IConnectionFactory>(_ => new ConnectionFactory
    {
        Uri = new Uri(rabbitMqConnectionString),
        ClientProvidedName = "ums-monitoring-healthchecks",
        AutomaticRecoveryEnabled = true,
        NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
        RequestedHeartbeat = TimeSpan.FromSeconds(30)
    });

    builder.Services.AddSingleton<IConnection>(sp =>
    {
        var connectionFactory = sp.GetRequiredService<IConnectionFactory>();
        return connectionFactory.CreateConnectionAsync().GetAwaiter().GetResult();
    });

    builder.Services.AddHealthChecks()
        .AddCheck(
            name: HealthCheckNames.Api,
            check: () => HealthCheckResult.Healthy("Monitoring HealthChecks API process is running."),
            tags: [HealthCheckTags.Live, HealthCheckTags.Ready, HealthCheckTags.Api])
        .AddSqlServer(
            connectionString: builder.Configuration.GetConnectionString("SqlServer")!,
            name: HealthCheckNames.InfrastructureSqlServer,
            failureStatus: HealthStatus.Unhealthy,
            tags: [HealthCheckTags.Ready, HealthCheckTags.Infrastructure, HealthCheckTags.Database, HealthCheckTags.SqlServer])
        .AddNpgSql(
            connectionString: builder.Configuration.GetConnectionString("Postgres")!,
            name: HealthCheckNames.InfrastructurePostgresSql,
            failureStatus: HealthStatus.Unhealthy,
            tags: [HealthCheckTags.Ready, HealthCheckTags.Infrastructure, HealthCheckTags.Database, HealthCheckTags.PostgresSql])
        .AddUrlGroup(
            uri: new Uri(builder.Configuration["HealthUrls:Loki"]!),
            name: HealthCheckNames.ObservabilityLoki,
            failureStatus: HealthStatus.Degraded,
            tags: [HealthCheckTags.Observability, HealthCheckTags.Logging])
        .AddUrlGroup(
            uri: new Uri(builder.Configuration["HealthUrls:Tempo"]!),
            name: HealthCheckNames.ObservabilityTempo,
            failureStatus: HealthStatus.Degraded,
            tags: [HealthCheckTags.Observability, HealthCheckTags.Tracing])
        .AddUrlGroup(
            uri: new Uri(builder.Configuration["HealthUrls:Prometheus"]!),
            name: HealthCheckNames.ObservabilityPrometheus,
            failureStatus: HealthStatus.Degraded,
            tags: [HealthCheckTags.Observability, HealthCheckTags.Metrics])
        .AddUrlGroup(
            uri: new Uri(builder.Configuration["HealthUrls:Grafana"]!),
            name: HealthCheckNames.ObservabilityGrafana,
            failureStatus: HealthStatus.Degraded,
            tags: [HealthCheckTags.Observability])
        .AddRabbitMQ(
            name: HealthCheckNames.InfrastructureRabbitMq,
            failureStatus: HealthStatus.Unhealthy,
            tags: [HealthCheckTags.Ready, HealthCheckTags.Infrastructure, HealthCheckTags.Broker, HealthCheckTags.RabbitMq],
            timeout: TimeSpan.FromSeconds(5));

    builder.Services
        .AddHealthChecksUI()
        .AddInMemoryStorage();

    var app = builder.Build();
    app.UseApplicationObservability();

    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("live"),
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    }).AllowAnonymous();

    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready"),
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    }).AllowAnonymous();

    app.MapHealthChecks("/health/infrastructure", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    }).AllowAnonymous();

    app.MapHealthChecksUI(options =>
    {
        options.UIPath = "/";
        options.ApiPath = "/api";
    });

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