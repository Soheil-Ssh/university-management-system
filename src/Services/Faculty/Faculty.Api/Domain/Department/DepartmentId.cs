namespace Faculty.Api.Domain.Department;

public sealed record DepartmentId(Guid Value)
{
    public static DepartmentId New() => new(Guid.NewGuid());
}