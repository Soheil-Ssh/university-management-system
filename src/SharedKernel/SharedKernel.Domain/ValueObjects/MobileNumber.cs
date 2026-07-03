using SharedKernel.Domain.Result;
using SharedKernel.Domain.ValueObjects.Errors;
using System.Text.RegularExpressions;

namespace SharedKernel.Domain.ValueObjects;

public sealed record MobileNumber
{
    private static readonly Regex Pattern = new(@"^(?:\+98|98|0)?(9\d{9})$", RegexOptions.Compiled);

    public string Value { get; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private MobileNumber() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private MobileNumber(string value)
    {
        Value = value;
    }

    public static Result<MobileNumber> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return MobileNumberErrors.Empty;

        value = value.Trim()
            .Replace(" ", "")
            .Replace("-", "")
            .Replace("(", "")
            .Replace(")", "");

        var match = Pattern.Match(value);

        if (!match.Success)
            return MobileNumberErrors.InvalidFormat;

        return new MobileNumber("0" + match.Groups[1].Value);
    }

    public override string ToString() => Value;
}