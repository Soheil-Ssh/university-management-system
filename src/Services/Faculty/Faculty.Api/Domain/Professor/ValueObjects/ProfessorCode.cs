using System.Text.RegularExpressions;

namespace Faculty.Api.Domain.Professor.ValueObjects;

public class ProfessorCode
{
    private static readonly Regex Pattern = new(@"^UMS_FAC_PROF_[0-9]{6}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private const string Prefix = "UMS_FAC_PROF";
    private const int NumberLength = 6;
    private const char Separator = '_';

    private const int MinNumber = 1;
    private const int MaxNumber = 999999;

    public string Value { get; }

#pragma warning disable CS8618
    private ProfessorCode() { }
#pragma warning restore CS8618

    private ProfessorCode(string value)
    {
        Value = value;
    }

    public static Result<ProfessorCode> Create(int number)
    {
        if (number is < MinNumber or > MaxNumber)
            return ProfessorCodeErrors.NumberOutOfRange;

        var value = $"{Prefix}{Separator}{number.ToString().PadLeft(NumberLength, '0')}";

        return new ProfessorCode(value);
    }

    public static Result<ProfessorCode> FromString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return ProfessorCodeErrors.Empty;

        value = value.Trim().ToUpperInvariant();

        if (!Pattern.IsMatch(value))
            return ProfessorCodeErrors.InvalidFormat;

        return new ProfessorCode(value);
    }

    internal static ProfessorCode FromPersistence(string value)
    {
        return new ProfessorCode(value);
    }

    public override string ToString() => Value;

    public static implicit operator string(ProfessorCode code) => code.Value;
}