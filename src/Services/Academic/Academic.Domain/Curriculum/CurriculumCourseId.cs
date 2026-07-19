namespace Academic.Domain.Curriculum;

public record CurriculumCourseId(Guid Value)
{
    public static CurriculumCourseId New() => new(Guid.NewGuid());
}