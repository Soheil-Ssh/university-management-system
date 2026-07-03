using SharedKernel.Domain.Result;
using System.Text.RegularExpressions;
using SharedKernel.Domain.ValueObjects.Errors;

namespace SharedKernel.Domain.ValueObjects;

public sealed record Email
{
    private static readonly Regex Pattern = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // ReSharper disable once MemberCanBePrivate.Global
    public string Value { get; init; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Email() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private Email(string value)
    {
        Value = value;
    }

    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return EmailErrors.Empty;

        value = value.Trim();

        if (value.Length > 256)
            return EmailErrors.TooLong;

        if (!Pattern.IsMatch(value))
            return EmailErrors.InvalidFormat;

        return new Email(value.ToLowerInvariant());
    }

    public override string ToString()
        => Value;
}