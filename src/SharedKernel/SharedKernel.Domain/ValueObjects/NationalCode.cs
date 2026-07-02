using SharedKernel.Domain.Result;
using System.Text.RegularExpressions;
using SharedKernel.Domain.ValueObjects.Errors;

namespace SharedKernel.Domain.ValueObjects;

public sealed record NationalCode
{
    private const int Length = 10;

    private static readonly Regex Pattern = new(@"^\d+$", RegexOptions.Compiled);

    public string Value { get; }

    private NationalCode(string value)
    {
        Value = value;
    }

    public static Result<NationalCode> Create(string? value)
    {
        value = Normalize(value);

        if (string.IsNullOrWhiteSpace(value))
            return NationalCodeErrors.Empty;

        if (value.Length != Length || !Pattern.IsMatch(value))
            return NationalCodeErrors.InvalidFormat;

        if (value.Distinct().Count() == 1)
            return NationalCodeErrors.InvalidPattern;

        if (!IsValidChecksum(value))
            return NationalCodeErrors.InvalidChecksum;

        return new NationalCode(value);
    }

    private static bool IsValidChecksum(string code)
    {
        var sum = 0;
        for (var i = 0; i < 9; i++)
        {
            var digit = code[i] - '0';
            var weight = 10 - i;
            sum += digit * weight;
        }

        var remainder = sum % 11;
        var controlDigit = code[9] - '0';

        if (remainder < 2)
            return controlDigit == remainder;
        else
            return controlDigit == 11 - remainder;
    }

    private static string? Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return Regex.Replace(value.Trim(), @"[\s\-/]", "");
    }

    public override string ToString() => Value;
}