namespace SharedKernel.Observability.Options;

public sealed class LokiOptions
{
    public bool Enabled { get; init; }
    public string Uri { get; init; } = "http://localhost:3100";
    public string MinimumLevel { get; init; } = "Information";
}