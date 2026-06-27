using System.Text.RegularExpressions;
using Identity.Api.Domain.User.Errors;

namespace Identity.Api.Domain.User.ValueObjects;

public sealed record Email
{
    private static readonly Regex Pattern = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    // ReSharper disable once MemberCanBePrivate.Global
    public string Value { get; init; }

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