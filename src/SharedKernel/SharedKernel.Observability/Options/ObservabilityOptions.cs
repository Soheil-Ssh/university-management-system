namespace SharedKernel.Observability.Options;

public sealed class ObservabilityOptions
{
    public const string SectionName = "Observability";
    public string ApplicationName { get; init; } = "UMS";
    public LokiOptions Loki { get; init; } = new();
    public MetricsOptions Metrics { get; init; } = new();
}