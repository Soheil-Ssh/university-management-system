using Identity.Api.Domain.Permission;

namespace Identity.Api.Domain.Role;

public class RolePermission : Entity<RolePermissionId>
{
    public RoleId RoleId { get; private set; }
    public PermissionId PermissionId { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private RolePermission() {}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private RolePermission(RolePermissionId id, RoleId roleId, PermissionId permissionId)
        : base(id)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }

    public static Result<RolePermission> Create(RoleId roleId, PermissionId permissionId)
        => new RolePermission(RolePermissionId.New(), roleId, permissionId);
}