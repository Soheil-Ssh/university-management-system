namespace SharedKernel.Domain.Identifiers;

public sealed record EmployeeId(Guid Value)
{
    public static EmployeeId New() => new(Guid.NewGuid());
}