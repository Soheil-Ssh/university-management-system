using System.Text.RegularExpressions;
using Academic.Domain.Curriculum.Errors;
using Academic.Domain.Major.ValueObjects;

namespace Academic.Domain.Curriculum.ValueObjects;

public sealed record CurriculumCode
{
    private static readonly Regex Pattern = new(@"^UMS_AC_MAJ_[0-9]{4}_CUR_[0-9]{4}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private const string Suffix = "CUR";
    private const int NumberLength = 4;
    private const char Separator = '_';
    private const int MinNumber = 1;
    private const int MaxNumber = 9999;

    public string Value { get; }

#pragma warning disable CS8618
    private CurriculumCode() { }
#pragma warning restore CS8618

    private CurriculumCode(string value)
    {
        Value = value;
    }

    public static Result<CurriculumCode> Create(
        MajorCode majorCode,
        int number)
    {
        if (number is < MinNumber or > MaxNumber)
            return CurriculumCodeErrors.NumberOutOfRange;

        var value = $"{majorCode.Value}{Separator}{Suffix}{Separator}{number.ToString().PadLeft(NumberLength, '0')}";

        return new CurriculumCode(value);
    }

    public static Result<CurriculumCode> FromString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return CurriculumCodeErrors.Empty;

        value = value.Trim().ToUpperInvariant();

        if (!Pattern.IsMatch(value))
            return CurriculumCodeErrors.InvalidFormat;

        return new CurriculumCode(value);
    }

    public override string ToString() => Value;

    public static implicit operator string(CurriculumCode code) => code.Value;
}