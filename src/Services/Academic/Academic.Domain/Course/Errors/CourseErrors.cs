namespace Academic.Domain.Course.Errors;

public static class CourseErrors
{
    // DepartmentId errors
    public static readonly Error DepartmentIdEmpty = new("Course.DepartmentId.Empty", "Department id is required.", ErrorType.Validation);

    // Title errors
    public static readonly Error TitleEmpty = new("Course.Title.Empty", "Course title is required.", ErrorType.Validation);
    public static readonly Error TitleTooLong = new("Course.Title.TooLong", "Course title is too long.", ErrorType.Validation);

    // TheoreticalCredits errors
    public static readonly Error TheoreticalCreditsInvalid = new("Course.TheoreticalCredits.Invalid", "Theoretical credits cannot be negative.", ErrorType.Validation);

    // PracticalCredits errors
    public static readonly Error PracticalCreditsInvalid = new("Course.PracticalCredits.Invalid", "Practical credits cannot be negative.", ErrorType.Validation);

    // TotalCredits errors
    public static readonly Error TotalCreditsInvalid = new("Course.TotalCredits.Invalid", "The total course credits must be greater than zero.", ErrorType.Validation);

    // Description errors
    public static readonly Error DescriptionTooLong = new("Course.Description.TooLong", "Course description is too long.", ErrorType.Validation);

    // Prerequisite errors
    public static readonly Error PrerequisiteAlreadyAdded = new("Course.Prerequisite.AlreadyAdded", "The prerequisite course has already been added.", ErrorType.Conflict);
    public static readonly Error PrerequisiteNotFound = new("Course.Prerequisite.NotFound", "The prerequisite course was not found.", ErrorType.Validation);
}