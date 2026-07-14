using System.Text.RegularExpressions;

namespace Faculty.Api.Domain.Department.ValueObjects;

public sealed record DepartmentCode
{
    private static readonly Regex Pattern = new(@"^UMS_FAC_DEP_[0-9]{4}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private const string Prefix = "UMS_FAC_DEP";
    private const int NumberLength = 4;
    private const char Separator = '_';

    private const int MinNumber = 1;
    private const int MaxNumber = 9999;

    public string Value { get; }

#pragma warning disable CS8618
    private DepartmentCode() { }
#pragma warning restore CS8618

    private DepartmentCode(string value)
    {
        Value = value;
    }

    public static Result<DepartmentCode> Create(int number)
    {
        if (number is < MinNumber or > MaxNumber)
            return FacultyCodeErrors.NumberOutOfRange;

        var value = $"{Prefix}{Separator}{number.ToString().PadLeft(NumberLength, '0')}";

        return new DepartmentCode(value);
    }

    public static Result<DepartmentCode> FromString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return FacultyCodeErrors.Empty;

        value = value.Trim().ToUpperInvariant();

        if (!Pattern.IsMatch(value))
            return FacultyCodeErrors.InvalidFormat;

        return new DepartmentCode(value);
    }

    internal static DepartmentCode FromPersistence(string value)
        => new DepartmentCode(value);

    public override string ToString() => Value;

    public static implicit operator string(DepartmentCode code) => code.Value;
}