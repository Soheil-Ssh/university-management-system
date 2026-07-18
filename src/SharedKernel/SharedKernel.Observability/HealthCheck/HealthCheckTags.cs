namespace SharedKernel.Observability.HealthCheck;

public static class HealthCheckTags
{
    public const string Live = "live";
    public const string Ready = "ready";

    public const string Api = "api";
    public const string Database = "database";
    public const string Broker = "broker";
    public const string Cache = "cache";
    public const string External = "external";
    public const string Observability = "observability";
    public const string Logging = "Logging";
    public const string Metrics = "Metrics";
    public const string Tracing = "Tracing";
    public const string Infrastructure = "infrastructure";

    public const string SqlServer = "sql-server";
    public const string PostgresSql = "postgresql";
    public const string RabbitMq = "rabbitmq";
    public const string Redis = "redis";
    public const string Grpc = "grpc";
}