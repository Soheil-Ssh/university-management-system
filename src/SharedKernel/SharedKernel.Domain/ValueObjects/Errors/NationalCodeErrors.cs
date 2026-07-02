using SharedKernel.Domain.Error;

namespace SharedKernel.Domain.ValueObjects.Errors;

public static class NationalCodeErrors
{
    public static readonly Error.Error Empty = new("NationalCode.Empty", "National code is required.", ErrorType.Validation);
    public static readonly Error.Error InvalidFormat = new("NationalCode.InvalidFormat", "National code must be 10 digits.", ErrorType.Validation);
    public static readonly Error.Error InvalidPattern = new("NationalCode.InvalidPattern", "National code cannot have all identical digits.", ErrorType.Validation);
    public static readonly Error.Error InvalidChecksum = new("NationalCode.InvalidChecksum", "National code has an invalid checksum.", ErrorType.Validation);
}