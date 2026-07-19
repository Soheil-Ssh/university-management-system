namespace Academic.Domain.Course;

public sealed record CourseId(Guid Value)
{
    public static CourseId New() => new(Guid.NewGuid());
}