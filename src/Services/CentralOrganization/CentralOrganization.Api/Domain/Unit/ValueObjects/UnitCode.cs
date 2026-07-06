using CentralOrganization.Api.Domain.Unit.Errors;
using System.Text.RegularExpressions;

namespace CentralOrganization.Api.Domain.Unit.ValueObjects;

public sealed record UnitCode
{
    private static readonly Regex Pattern = new(@"^UMS_CO_[0-9]{4}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private const string Prefix = "UMS_CO";
    private const int NumberLength = 4;
    private const char Separator = '_';

    private const int MinNumber = 1;
    private const int MaxNumber = 9999;

    public string Value { get; }

#pragma warning disable CS8618
    private UnitCode() { }
#pragma warning restore CS8618

    private UnitCode(string value)
    {
        Value = value;
    }

    public static Result<UnitCode> Create(int number)
    {
        if (number is < MinNumber or > MaxNumber)
            return UnitCodeErrors.NumberOutOfRange;

        var value = $"{Prefix}{Separator}{number.ToString().PadLeft(NumberLength, '0')}";

        return new UnitCode(value);
    }

    public static Result<UnitCode> FromString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return UnitCodeErrors.Empty;

        value = value.Trim().ToUpperInvariant();

        if (Pattern.IsMatch(value))
            return UnitCodeErrors.InvalidFormat;

        return new UnitCode(value);
    }

    public override string ToString() => Value;

    public static implicit operator string(UnitCode code) => code.Value;
}