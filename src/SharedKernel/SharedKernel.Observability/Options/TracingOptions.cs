namespace SharedKernel.Observability.Options;

public sealed class TracingOptions
{
    public bool Enabled { get; init; }
    public string Endpoint { get; init; } = "http://localhost:4318/v1/traces";
    public double SamplingRatio { get; init; } = 1.0;
    public string[] ExcludedPathPrefixes { get; init; } = ["/health", "/health-checks-api"];
}