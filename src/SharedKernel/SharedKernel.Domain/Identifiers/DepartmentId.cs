namespace SharedKernel.Domain.Identifiers;

public sealed record DepartmentId(Guid Value)
{
    public static DepartmentId New() => new(Guid.NewGuid());
}