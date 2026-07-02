using System.Text.RegularExpressions;
using File.Api.Domain.File.Errors;

namespace File.Api.Domain.File.ValueObjects;

public sealed record FileHash
{
    private static readonly Regex Pattern = new("^[a-fA-F0-9]{64}$", RegexOptions.Compiled);

    public string Value { get; }

    private FileHash(string value)
    {
        Value = value.ToLowerInvariant();
    }

    public static Result<FileHash> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return FileHashErrors.Empty;

        value = value.Trim();

        if (!Pattern.IsMatch(value))
            return FileHashErrors.InvalidFormat;

        return new FileHash(value);
    }

    public override string ToString() => Value;
}