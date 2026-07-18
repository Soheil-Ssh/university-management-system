using Academic.Domain.Major.Errors;
using System.Text.RegularExpressions;

namespace Academic.Domain.Major.ValueObjects;

public sealed record MajorCode
{
    private static readonly Regex Pattern = new(@"^UMS_AC_MAJ_[0-9]{4}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private const string Prefix = "UMS_AC_MAJ";
    private const int NumberLength = 4;
    private const char Separator = '_';

    private const int MinNumber = 1;
    private const int MaxNumber = 9999;

    public string Value { get; }

#pragma warning disable CS8618
    private MajorCode() { }
#pragma warning restore CS8618

    private MajorCode(string value)
    {
        Value = value;
    }

    public static Result<MajorCode> Create(int number)
    {
        if (number is < MinNumber or > MaxNumber)
            return MajorCodeErrors.NumberOutOfRange;

        var value = $"{Prefix}{Separator}{number.ToString().PadLeft(NumberLength, '0')}";

        return new MajorCode(value);
    }

    public static Result<MajorCode> FromString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return MajorCodeErrors.Empty;

        value = value.Trim().ToUpperInvariant();

        if (!Pattern.IsMatch(value))
            return MajorCodeErrors.InvalidFormat;

        return new MajorCode(value);
    }

    public override string ToString() => Value;

    public static implicit operator string(MajorCode code) => code.Value;
}