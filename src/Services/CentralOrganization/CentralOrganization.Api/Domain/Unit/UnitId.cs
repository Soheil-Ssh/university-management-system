namespace CentralOrganization.Api.Domain.Unit;

public sealed record UnitId(Guid Value)
{
    public static UnitId New() => new(Guid.NewGuid());
}