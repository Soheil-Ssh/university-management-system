using SharedKernel.Domain.Result;
using SharedKernel.Domain.ValueObjects.Errors;
using System.Text;
using System.Text.RegularExpressions;

namespace SharedKernel.Domain.ValueObjects;

public sealed record Name
{
    private static readonly Regex Pattern = new(@"^[\p{L}\p{M}\s.'-]+$", RegexOptions.Compiled);

    private const int MinLength = 2;
    private const int MaxLength = 100;

    public string Value { get; }

    private Name(string value)
    {
        Value = value;
    }

    public static Result<Name> Create(string? value)
    {
        value = Normalize(value);

        if (string.IsNullOrWhiteSpace(value))
            return NameErrors.Empty;

        if (value.Length < MinLength)
            return NameErrors.TooShort;

        if (value.Length > MaxLength)
            return NameErrors.TooLong;

        if (!Pattern.IsMatch(value))
            return NameErrors.InvalidCharacters;

        return new Name(value);
    }

    private static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        value = value
            .Normalize(NormalizationForm.FormC)
            .Trim();

        value = Regex.Replace(value, @"\s+", " ");

        return value;
    }

    public override string ToString()
        => Value;
}