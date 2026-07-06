using System.Text.RegularExpressions;
using CentralOrganization.Api.Domain.Employee.Errors;

namespace CentralOrganization.Api.Domain.Employee.ValueObjects;

public sealed record PersonnelCode
{
    private const string Prefix = "UMS_CO_EMP";
    private const int NumberLength = 6;
    private const char Separator = '_';

    private const int MinNumber = 1;
    private const int MaxNumber = 999999;

    private static readonly Regex Pattern = new(@"^UMS_CO_EMP_[0-9]{4}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

#pragma warning disable CS8618
    private PersonnelCode() { }
#pragma warning restore CS8618

    private PersonnelCode(string value)
    {
        Value = value;
    }

    public static Result<PersonnelCode> Create(int number)
    {
        if (number is < MinNumber or > MaxNumber)
            return PersonnelCodeErrors.NumberOutOfRange;

        var value = $"{Prefix}{Separator}{number.ToString().PadLeft(NumberLength, '0')}";

        return new PersonnelCode(value);
    }

    public static Result<PersonnelCode> FromString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return PersonnelCodeErrors.Empty;

        value = value.Trim().ToUpperInvariant();

        if (!Pattern.IsMatch(value))
            return PersonnelCodeErrors.InvalidFormat;

        return new PersonnelCode(value);
    }

    public override string ToString() => Value;

    public static implicit operator string(PersonnelCode code) => code.Value;
}