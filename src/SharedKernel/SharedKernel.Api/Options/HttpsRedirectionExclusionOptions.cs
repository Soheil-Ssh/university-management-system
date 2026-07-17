namespace SharedKernel.Api.Options;

public sealed class HttpsRedirectionExclusionOptions
{
    public List<string> ExcludedPathPrefixes { get; } =
    [
        "/health",
        "/metrics"
    ];
}