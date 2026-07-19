namespace Academic.Domain.Course;

public sealed record CoursePrerequisiteId(Guid Value)
{
    public static CoursePrerequisiteId New() => new(Guid.NewGuid());
}