namespace SharedKernel.Domain.Error;

public sealed record Error(
    string Code,
    string? Description = null,
    ErrorType Type = ErrorType.None,
    string? Path = null,
    IReadOnlyDictionary<string, object?>? Metadata = null)
{
    public static readonly Error None = new(string.Empty);

    public Error WithPath(string path)
        => this with { Path = path };

    public Error WithMetadata(string key, object? value)
    {
        var metadata = Metadata is null
            ? new Dictionary<string, object?>()
            : new Dictionary<string, object?>(Metadata);

        metadata[key] = value;

        return this with { Metadata = metadata };
    }

    public override string ToString()
    {
        var property = string.IsNullOrWhiteSpace(Path)
            ? string.Empty
            : $" ({Path})";

        return string.IsNullOrWhiteSpace(Description)
            ? $"{Code}{property}"
            : $"{Code}{property}: {Description}";
    }
}