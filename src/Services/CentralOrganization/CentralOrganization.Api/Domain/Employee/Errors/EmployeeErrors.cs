namespace CentralOrganization.Api.Domain.Employee.Errors;

public class EmployeeErrors
{
    public static readonly Error NotFound = new("Employee.NotFound", "The employee was not found.", ErrorType.Unauthorized);
    public static readonly Error IdentityAlreadyProvisioned = 
        new("Employee.IdentityAlreadyProvisioned", "The employee identity has already been provisioned.", ErrorType.Conflict);
    public static readonly Error BirthDateInvalid = new("Employee.BirthDate.Invalid", "Employee birth date is invalid.", ErrorType.Validation);
    public static readonly Error GenderInvalid = new("Employee.Gender.Invalid", "Employee gender is invalid.", ErrorType.Validation);
    public static readonly Error EducationFieldEmpty = new("Employee.EducationField.Empty", "Employee education cannot be empty.", ErrorType.Validation);
    public static readonly Error EducationFieldTooLong = new("Employee.EducationField.TooLong", "Employee education field must not exceed 150 characters.", ErrorType.Validation);
    public static readonly Error JobTitleEmpty = new("Employee.JobTitle.Empty", "Employee job cannot be empty.", ErrorType.Validation);
    public static readonly Error JobTitleTooLong = new("Employee.JobTitle.TooLong", "Employee job title must not exceed 150 characters.", ErrorType.Validation);
    public static readonly Error IdentityUserIdInvalid = new("Employee.IdentityUserId.Invalid", "Employee identity user id is invalid.", ErrorType.Validation);
    public static readonly Error ProfileImageFileIdInvalid = new("Employee.ProfileImageFileId.Invalid", "Employee profile image file id is invalid.", ErrorType.Validation);
    public static readonly Error PersonnelCodeAlreadyExists = 
        new("Employee.PersonnelCode.AlreadyExists", "An employee with the specified personnel code already exists.", ErrorType.Conflict);
    public static readonly Error NationalCodeAlreadyExists = 
        new("Employee.NationalCode.AlreadyExists", "An employee with the specified national code already exists.", ErrorType.Conflict);
    public static readonly Error MobileNumberAlreadyExists = new(
        "Employee.MobileNumber.AlreadyExists", "An employee with the specified mobile number already exists.", ErrorType.Conflict);
    public static readonly Error EmailAlreadyExists = new(
        "Employee.Email.AlreadyExists", "An employee with the specified email already exists.", ErrorType.Conflict);
}