namespace Academic.Domain.Major;

public sealed record MajorId(Guid Value)
{
    public static MajorId New() => new(Guid.NewGuid());
}