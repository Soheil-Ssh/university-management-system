using SharedKernel.Domain.Result;
using System.Text.RegularExpressions;
using SharedKernel.Domain.ValueObjects.Errors;

namespace SharedKernel.Domain.ValueObjects;

public sealed record PhoneNumber
{
    private static readonly Regex Pattern = new(@"^(?:\+98|98|0)?([1-9]\d{1,2})(\d{8})$",
        RegexOptions.Compiled);

    public string Value { get; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private PhoneNumber() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static Result<PhoneNumber> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return PhoneNumberErrors.Empty;

        value = value.Trim()
            .Replace(" ", "")
            .Replace("-", "")
            .Replace("(", "")
            .Replace(")", "");

        var match = Pattern.Match(value);

        if (!match.Success)
            return PhoneNumberErrors.InvalidFormat;

        var areaCode = match.Groups[1].Value;
        var subscriber = match.Groups[2].Value;

        return new PhoneNumber($"0{areaCode}{subscriber}");
    }

    public override string ToString() => Value;
}