using SharedKernel.Domain.Result;
using SharedKernel.Domain.ValueObjects.Errors;
using System.Text.RegularExpressions;

namespace SharedKernel.Domain.ValueObjects;

public sealed record PostalCode
{
    private static readonly Regex Pattern = new(@"^\d{10}$", RegexOptions.Compiled);

    public string Value { get; }

    private PostalCode(string value)
    {
        Value = value;
    }

    public static Result<PostalCode> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return PostalCodeErrors.Empty;

        value = Normalize(value);

        if (!Pattern.IsMatch(value))
            return PostalCodeErrors.InvalidFormat;

        if (value.Distinct().Count() == 1)
            return PostalCodeErrors.Invalid;

        return new PostalCode(value);
    }

    private static string Normalize(string value)
        => value.Trim().Replace(" ", "").Replace("-", "");

    public override string ToString()
        => Value;
}