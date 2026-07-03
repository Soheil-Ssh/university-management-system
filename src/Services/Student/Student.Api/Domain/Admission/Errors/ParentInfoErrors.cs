using SharedKernel.Domain.Error;

namespace Student.Api.Domain.Admission.Errors;

public static class ParentInfoErrors
{
    public static readonly Error Required = new("ParentInfo.Required", "Parent information is required.", ErrorType.Validation);

    public static readonly Error FirstNameInvalid = new("ParentInfo.FirstName.Invalid", "Invalid first name.", ErrorType.Validation);
    public static readonly Error LastNameInvalid = new("ParentInfo.LastName.Invalid", "Invalid last name.", ErrorType.Validation);
    public static readonly Error NationalCodeInvalid = new("ParentInfo.NationalCode.Invalid", "Invalid national code.", ErrorType.Validation);
    public static readonly Error MobileInvalid = new("ParentInfo.Mobile.Invalid", "Invalid mobile number.", ErrorType.Validation);
}