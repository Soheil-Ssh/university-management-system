using Academic.Domain.Course.Errors;

namespace Academic.Domain.Course;

public sealed class CoursePrerequisite : Entity<CoursePrerequisiteId>
{
    public CourseId CourseId { get; private set; }
    public CourseId PrerequisiteCourseId { get; private set; }

#pragma warning disable CS8618
    private CoursePrerequisite() { }
#pragma warning restore CS8618

    private CoursePrerequisite(CourseId courseId, CourseId prerequisiteCourseId)
    {
        CourseId = courseId;
        PrerequisiteCourseId = prerequisiteCourseId;
    }

    internal static Result<CoursePrerequisite> Create(CourseId courseId, CourseId prerequisiteCourseId)
    {
        if (courseId.Value == Guid.Empty)
            return CoursePrerequisiteErrors.CourseIdEmpty;

        if (prerequisiteCourseId.Value == Guid.Empty)
            return CoursePrerequisiteErrors.PrerequisiteCourseIdEmpty;

        if (courseId == prerequisiteCourseId)
            return CoursePrerequisiteErrors.CourseCannotBeOwnPrerequisite;

        return new CoursePrerequisite(courseId, prerequisiteCourseId);
    }
}