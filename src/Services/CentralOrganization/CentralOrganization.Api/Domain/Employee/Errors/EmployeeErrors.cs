namespace CentralOrganization.Api.Domain.Employee.Errors;

public class EmployeeErrors
{
    public static readonly Error BirthDateInvalid = new("Employee.BirthDate.Invalid", "Employee birth date is invalid.");
    public static readonly Error GenderInvalid = new("Employee.Gender.Invalid", "Employee gender is invalid.");
    public static readonly Error EducationFieldEmpty = new("Employee.EducationField.Empty", "Employee education cannot be empty.");
    public static readonly Error EducationFieldTooLong = new("Employee.EducationField.TooLong", "Employee education field must not exceed 150 characters.");
    public static readonly Error JobTitleEmpty = new("Employee.JobTitle.Empty", "Employee job cannot be empty.");
    public static readonly Error JobTitleTooLong = new("Employee.JobTitle.TooLong", "Employee job title must not exceed 150 characters.");
    public static readonly Error IdentityUserIdInvalid = new("Employee.IdentityUserId.Invalid", "Employee identity user id is invalid.");
    public static readonly Error ProfileImageFileIdInvalid = new("Employee.ProfileImageFileId.Invalid", "Employee profile image file id is invalid.");
}