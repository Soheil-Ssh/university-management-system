using System.Text.RegularExpressions;

namespace Faculty.Api.Domain.Faculty.ValueObjects;

public sealed record FacultyCode
{
    private static readonly Regex Pattern = new(@"^UMS_FAC_FACULTY_[0-9]{4}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private const string Prefix = "UMS_FAC_FACULTY";
    private const int NumberLength = 4;
    private const char Separator = '_';

    private const int MinNumber = 1;
    private const int MaxNumber = 9999;

    public string Value { get; }

#pragma warning disable CS8618
    private FacultyCode() {}
#pragma warning restore CS8618

    private FacultyCode(string value)
    {
        Value = value;
    }

    public static Result<FacultyCode> Create(int number)
    {
        if (number is < MinNumber or > MaxNumber)
            return FacultyCodeErrors.NumberOutOfRange;

        var value = $"{Prefix}{Separator}{number.ToString().PadLeft(NumberLength, '0')}";

        return new FacultyCode(value);
    }

    public static Result<FacultyCode> FromString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return FacultyCodeErrors.Empty;

        value = value.Trim().ToUpperInvariant();

        if (!Pattern.IsMatch(value))
            return FacultyCodeErrors.InvalidFormat;

        return new FacultyCode(value);
    }

    internal static FacultyCode FromPersistence(string value)
    {
        return new FacultyCode(value);
    }

    public override string ToString() => Value;

    public static implicit operator string(FacultyCode code) => code.Value;
}