using SharedKernel.Domain.Error;

namespace SharedKernel.Domain.ValueObjects.Errors;

public class PostalCodeErrors
{
    public static readonly Error.Error Empty = new("PostalCode.Empty", "Postal code is required.", ErrorType.Validation);
    public static readonly Error.Error InvalidFormat = 
        new("PostalCode.InvalidFormat", "Postal code must contain exactly 10 digits.", ErrorType.Validation);
    public static readonly Error.Error Invalid = new("PostalCode.Invalid", "Postal code is invalid.", ErrorType.Validation);
}