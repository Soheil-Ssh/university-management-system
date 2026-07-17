namespace SharedKernel.Observability.Options;

public sealed class LokiOptions
{
    public bool Enabled { get; init; } = false;
    public string Uri { get; init; } = "http://localhost:3100";
    public string MinimumLevel { get; init; } = "Information";
}