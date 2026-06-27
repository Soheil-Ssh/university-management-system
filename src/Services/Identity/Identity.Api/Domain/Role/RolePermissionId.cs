namespace Identity.Api.Domain.Role;

public record RolePermissionId(Guid Value)
{
    public static RolePermissionId New() => new(Guid.NewGuid());
}