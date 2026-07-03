namespace File.Api.Infrastructure.Persistence.Options;

public sealed record FileStorageOptions
{
    public const string SectionName = "FileStorage";
    public string RootPath { get; init; } = string.Empty;
}