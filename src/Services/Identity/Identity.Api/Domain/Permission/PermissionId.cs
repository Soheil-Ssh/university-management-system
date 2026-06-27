namespace Identity.Api.Domain.Permission;

public sealed record PermissionId(Guid Value)
{
    public static PermissionId New() => new(Guid.NewGuid());
}