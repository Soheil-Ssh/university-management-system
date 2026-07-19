namespace Academic.Domain.Curriculum.Errors;

public static class CurriculumCourseErrors
{
    public static readonly Error CurriculumIdEmpty = new("CurriculumCourse.CurriculumId.Empty", "Curriculum id is required.");
    public static readonly Error CourseIdEmpty = new("CurriculumCourse.CourseId.Empty", "Course id is required.");
    public static readonly Error CategoryInvalid = new("CurriculumCourse.Category.Invalid", "Curriculum course category is invalid.");
    public static readonly Error RequirementTypeInvalid = new("CurriculumCourse.RequirementType.Invalid", "Curriculum course requirement type is invalid.");
    public static readonly Error SuggestedSemesterInvalid = new("CurriculumCourse.SuggestedSemester.Invalid", "Suggested semester must be greater than zero.");
    public static readonly Error DisplayOrderInvalid = new("CurriculumCourse.DisplayOrder.Invalid", "Display order must be greater than zero when specified.");
}