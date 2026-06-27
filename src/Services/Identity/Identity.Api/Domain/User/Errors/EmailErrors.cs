namespace Identity.Api.Domain.User.Errors;

public static class EmailErrors
{
    public static readonly Error Empty = new("Email.Empty", "Email cannot be empty.", ErrorType.Validation);
    public static readonly Error InvalidFormat = new("Email.InvalidFormat", "Email is not in a valid format.", ErrorType.Validation);
    public static readonly Error TooLong = new("Email.TooLong", "Email length is too long.", ErrorType.Validation);
}