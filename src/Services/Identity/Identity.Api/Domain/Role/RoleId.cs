namespace Identity.Api.Domain.Role;

public sealed record RoleId(Guid Value)
{
    public static RoleId New() => new(Guid.NewGuid());
}