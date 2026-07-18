namespace SharedKernel.Observability.HealthCheck;

public static class HealthCheckNames
{
    public const string Api = "api";

    public const string DatabaseSqlServer = "database-sql-server";
    public const string DatabasePostgresSql = "database-postgresql";

    public const string MessageBrokerRabbitMq = "message-broker-rabbitmq";

    public const string CacheRedis = "cache-redis";

    public const string ExternalFileGrpc = "external-file-grpc";
    public const string ExternalIdentityApi = "external-identity-api";

    public const string ObservabilityLoki = "observability-loki";
    public const string ObservabilityTempo = "observability-tempo";
    public const string ObservabilityPrometheus = "observability-prometheus";
    public const string ObservabilityGrafana = "observability-grafana";

    public const string InfrastructureSqlServer = "sql-server";
    public const string InfrastructurePostgresSql = "postgresql";
    public const string InfrastructureRabbitMq = "rabbitmq";
}