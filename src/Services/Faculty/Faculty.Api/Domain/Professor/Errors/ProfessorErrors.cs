namespace Faculty.Api.Domain.Professor.Errors;

public static class ProfessorErrors
{
    // General errors
    public static readonly Error NotFound = new("Professor.NotFound", "Professor was not found.", ErrorType.NotFound);

    // Code errors
    public static readonly Error CodeRequired = new("Professor.Code.Required", "Professor code is required.", ErrorType.Validation);

    // Specialization errors
    public static readonly Error SpecializationEmpty = new("Professor.Specialization.Empty", "Professor specialization cannot be empty.", ErrorType.Validation);
    public static readonly Error SpecializationTooLong = new("Professor.Specialization.TooLong", "Professor specialization cannot exceed 150 characters.", ErrorType.Validation);

    // Academic rank errors
    public static readonly Error AcademicRankInvalid = new("Professor.AcademicRank.Invalid", "Professor academic rank is invalid.", ErrorType.Validation);

    // Employment errors
    public static readonly Error EmploymentTypeInvalid = new("Professor.EmploymentType.Invalid", "Professor employment type is invalid.", ErrorType.Validation);
    public static readonly Error EmploymentStartDateInvalid = new("Professor.EmploymentStartDate.Invalid", "Professor employment start date is invalid.", ErrorType.Validation);

    // Profile image errors
    public static readonly Error ProfileImageFileIdInvalid = new("Professor.ProfileImageFileId.Invalid", "Professor profile image file id is invalid.", ErrorType.Validation);

    // Identity provisioning errors
    public static readonly Error IdentityUserIdInvalid = new("Professor.IdentityUserId.Invalid", "Identity user id is invalid.", ErrorType.Validation);
    public static readonly Error IdentityAlreadyProvisioned = new("Professor.Identity.AlreadyProvisioned", "Identity user has already been provisioned for this professor.", ErrorType.Conflict);
    public static readonly Error IdentityNotProvisioned = new("Professor.Identity.NotProvisioned", "Professor identity user has not been provisioned successfully.", ErrorType.Conflict);

    // Unique errors
    public static readonly Error NationalCodeAlreadyExists = new("Professor.NationalCode.AlreadyExists", "National code is already registered for another professor.", ErrorType.Conflict);
    public static readonly Error EmailAlreadyExists = new("Professor.Email.AlreadyExists", "Email is already registered for another professor.", ErrorType.Conflict);
    public static readonly Error MobileNumberAlreadyExists = new("Professor.MobileNumber.AlreadyExists", "Mobile number is already registered for another professor.", ErrorType.Conflict);
}