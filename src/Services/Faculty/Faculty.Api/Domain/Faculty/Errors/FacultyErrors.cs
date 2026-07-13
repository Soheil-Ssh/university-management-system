using SharedKernel.Domain.Error;

namespace Faculty.Api.Domain.Faculty.Errors;

public static class FacultyErrors
{
    // General Errors
    public static readonly Error NotFound = new("Faculty.NotFound", "Faculty was not found.", ErrorType.NotFound);
    public static readonly Error Inactive = new("Faculty.Inactive", "The faculty is inactive.", ErrorType.Conflict);
    public static readonly Error CannotAssignDeanToInactiveFaculty = 
        new("Faculty.CannotAssignDeanToInactiveFaculty", "A dean cannot be assigned to an inactive faculty.", ErrorType.Conflict);

    // Name Errors
    public static readonly Error NameEmpty = new("Faculty.Name.Empty", "Faculty name cannot be empty.", ErrorType.Validation);
    public static readonly Error NameTooLong = new("Faculty.Name.TooLong", "Faculty name cannot exceed 150 characters.", ErrorType.Validation);
    public static readonly Error NameAlreadyExists = new("Faculty.Name.AlreadyExists", "A faculty with the specified name already exists.", ErrorType.Conflict);

    // Description Errors
    public static readonly Error DescriptionTooLong = new("Faculty.Description.TooLong", "Faculty description cannot exceed 500 characters.", ErrorType.Validation);

    // Code Errors
    public static readonly Error CodeAlreadyExists = new("Faculty.Code.AlreadyExists", "A faculty with the specified code already exists.", ErrorType.Conflict);

    // Dean Professor Errors
    public static readonly Error DeanProfessorIdEmpty = new("Faculty.DeanProfessorId.Empty", "Dean professor identifier cannot be empty.", ErrorType.Validation);
}