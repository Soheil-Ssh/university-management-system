namespace Academic.Domain.Course.Errors;

public static class CoursePrerequisiteErrors
{
    public static readonly Error CourseIdEmpty = new("CoursePrerequisite.CourseId.Empty", "Course id is required.", ErrorType.Validation);
    public static readonly Error PrerequisiteCourseIdEmpty = new("CoursePrerequisite.PrerequisiteCourseId.Empty", "Prerequisite course id is required.", ErrorType.Validation);
    public static readonly Error CourseCannotBeOwnPrerequisite = 
        new("CoursePrerequisite.CourseCannotBeOwnPrerequisite", "A course cannot be its own prerequisite.", ErrorType.Validation);
}