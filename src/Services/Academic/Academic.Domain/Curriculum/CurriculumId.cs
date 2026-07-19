namespace Academic.Domain.Curriculum;

public sealed record CurriculumId(Guid Value)
{
    public static CurriculumId New() => new(Guid.NewGuid());
}