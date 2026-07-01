using SharedKernel.Domain.Error;

namespace SharedKernel.Domain.ValueObjects.Errors;

public static class NameErrors
{
    public static readonly Error.Error Empty = new("Name.Empty", "Name is required.", ErrorType.Validation);
    public static readonly Error.Error TooShort = new("Name.TooShort", "Name is too short.", ErrorType.Validation);
    public static readonly Error.Error TooLong = new("Name.TooLong", "Name is too long.", ErrorType.Validation);
    public static readonly Error.Error InvalidCharacters = new("Name.InvalidCharacters", "Name contains invalid characters.", ErrorType.Validation);
}