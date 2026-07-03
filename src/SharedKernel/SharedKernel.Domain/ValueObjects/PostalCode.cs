using SharedKernel.Domain.Result;
using SharedKernel.Domain.ValueObjects.Errors;
using System.Text.RegularExpressions;

namespace SharedKernel.Domain.ValueObjects;

public sealed record PostalCode
{
    private static readonly Regex Pattern = new(@"^\d{10}$", RegexOptions.Compiled);

    public string Value { get; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private PostalCode() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

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