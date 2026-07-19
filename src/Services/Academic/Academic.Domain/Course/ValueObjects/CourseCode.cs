using System.Text.RegularExpressions;
using Academic.Domain.Course.Errors;

namespace Academic.Domain.Course.ValueObjects;

public sealed record CourseCode
{
    private static readonly Regex Pattern = new(@"^UMS_AC_CRS_[0-9]{6}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private const string Prefix = "UMS_AC_CRS";
    private const int NumberLength = 6;
    private const char Separator = '_';

    private const int MinNumber = 1;
    private const int MaxNumber = 999999;

    public string Value { get; }

#pragma warning disable CS8618
    private CourseCode() { }
#pragma warning restore CS8618

    private CourseCode(string value)
    {
        Value = value;
    }

    public static Result<CourseCode> Create(int number)
    {
        if (number is < MinNumber or > MaxNumber)
            return CourseCodeErrors.NumberOutOfRange;

        var value = $"{Prefix}{Separator}{number.ToString().PadLeft(NumberLength, '0')}";

        return new CourseCode(value);
    }

    public static Result<CourseCode> FromString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return CourseCodeErrors.Empty;

        value = value.Trim().ToUpperInvariant();

        if (!Pattern.IsMatch(value))
            return CourseCodeErrors.InvalidFormat;

        return new CourseCode(value);
    }

    public override string ToString() => Value;

    public static implicit operator string(CourseCode code) => code.Value;
}