namespace SharedKernel.Observability.Options;

public class MetricsOptions
{
    public bool Enabled { get; init; } = false;
    public string EndpointPath { get; init; } = "/metrics";
}