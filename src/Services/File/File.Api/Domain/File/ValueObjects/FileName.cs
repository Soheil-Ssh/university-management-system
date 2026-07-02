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

        value = value.Trim();

        if (value.Length > MaxLength)
            return FileNameErrors.TooLong;

        return new FileName(value);
    }

    public override string ToString() => Value;
}