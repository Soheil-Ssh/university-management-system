using SharedKernel.Domain.Error;

namespace Student.Api.Domain.Admission.Errors;

public static class TrackingCodeErrors
{
    public static readonly Error Empty = new("TrackingCode.Empty", "Tracking code cannot be empty.", ErrorType.Validation);
    public static readonly Error  InvalidFormat = new("TrackingCode.InvalidFormat", "Invalid tracking code format.", ErrorType.Validation);
}