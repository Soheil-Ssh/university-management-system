using SharedKernel.Domain.Error;

namespace SharedKernel.Domain.ValueObjects.Errors;

public class MobileNumberErrors
{
    public static readonly Error.Error Empty = new("PhoneNumber.Empty", "Phone number cannot be empty.", ErrorType.Validation);
    public static readonly Error.Error InvalidFormat = new("PhoneNumber.InvalidFormat", "Phone number  is not in a valid format.", ErrorType.Validation);
    public static readonly Error.Error TooLong = new("PhoneNumber.TooLong", "Phone number  length is too long.", ErrorType.Validation);
}