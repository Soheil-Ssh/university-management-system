using Identity.Api.Domain.Role.Errors;

namespace Identity.Api.Domain.Role;

public sealed class Role : AggregateRoot<RoleId>
{
    public string Name { get; private set; }
    public string? Description { get; private set; }

    private readonly List<RolePermission> _rolePermissions = [];
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Role() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private Role(RoleId id, string name, string? description = null) : base(id)
    {
        Name = name;
        Description = description;
    }

    public static Result<Role> Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return RoleErrors.NameEmpty;

        name = name.Trim();

        if (name.Length < 2)
            return RoleErrors.NameTooShort;
        if (name.Length > 100)
            return RoleErrors.NameTooLong;

        if (!string.IsNullOrWhiteSpace(description) && description.Length > 500)
            return RoleErrors.DescriptionTooLong;

        return new Role(RoleId.New(), name, description);
    }

    public Result UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return RoleErrors.NameEmpty;

        name = name.Trim();

        if (name.Length < 2)
            return RoleErrors.NameTooShort;
        if (name.Length > 100)
            return RoleErrors.NameTooLong;

        Name = name;

        return Result.Success();
    }

    public Result UpdateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return RoleErrors.DescriptionEmpty;
        if (description.Length > 500)
            return RoleErrors.DescriptionTooLong;

        Description = description;

        return Result.Success();
    }

    public Result AddPermission(PermissionId permissionId)
    {
        if (_rolePermissions.Any(x => x.PermissionId == permissionId))
            return RoleErrors.PermissionAlreadyExist;

        var rolePermissionResult = RolePermission.Create(Id, permissionId);
        if (rolePermissionResult.IsFailure)
            return rolePermissionResult.Error;

        _rolePermissions.Add(rolePermissionResult.Data);

        return Result.Success();
    }

    public Result RemovePermission(PermissionId permissionId)
    {
        var permission = _rolePermissions
            .FirstOrDefault(x => x.PermissionId == permissionId);

        if (permission is null)
            return RoleErrors.PermissionNotFound;

        _rolePermissions.Remove(permission);

        return Result.Success();
    }
}