namespace Faculty.Api.Domain.Faculty;

public sealed record FacultyId(Guid Value)
{
    public static FacultyId New() => new(Guid.NewGuid());
}