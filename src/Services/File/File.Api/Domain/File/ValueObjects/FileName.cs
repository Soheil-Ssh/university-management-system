using File.Api.Domain.File.Errors;

namespace File.Api.Domain.File.ValueObjects;

public sealed record FileName
{
    private const int MaxLength = 255;

    public string Value { get; }
    public string Extension => Path.GetExtension(Value);
    public string NameWithoutExtension => Path.GetFileNameWithoutExtension(Value);

    private FileName(string value)
    {
        Value = value;
    }

    public static Result<FileName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return FileNameErrors.Empty;

        value = Path.GetFileName(value.Trim());

        if (string.IsNullOrWhiteSpace(value))
            return FileNameErrors.Empty;

        if (value.Length > MaxLength)
            return FileNameErrors.TooLong;

        if (value.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            return FileNameErrors.ContainsInvalidChars;

        return new FileName(value);
    }

    public static Result<FileName> CreateFromNameWithoutExtension(string nameWithoutExtension, string currentExtension)
    {
        if (string.IsNullOrWhiteSpace(nameWithoutExtension))
            return FileNameErrors.Empty;

        nameWithoutExtension = nameWithoutExtension.Trim();

        if (nameWithoutExtension.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            return FileNameErrors.Invalid;

        if (Path.HasExtension(nameWithoutExtension))
            return FileNameErrors.ExtensionNotAllowed;

        currentExtension = currentExtension?.Trim() ?? string.Empty;

        string newValue = $"{nameWithoutExtension}{currentExtension}";

        if (newValue.Length > MaxLength)
            return FileNameErrors.TooLong;

        return new FileName(newValue);
    }

    public override string ToString() => Value;
}