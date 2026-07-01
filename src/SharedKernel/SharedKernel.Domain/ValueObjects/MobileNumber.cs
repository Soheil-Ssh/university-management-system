using SharedKernel.Domain.Result;
using SharedKernel.Domain.ValueObjects.Errors;
using System.Text.RegularExpressions;

namespace SharedKernel.Domain.ValueObjects;

public sealed record MobileNumber
{
    private static readonly Regex Pattern = new(@"^(?:\+98|98|0)?(9\d{9})$", RegexOptions.Compiled);

    public string Value { get; }

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