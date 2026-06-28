namespace Identity.Api.Domain.Role;

public sealed class Role : AggregateRoot<RoleId>
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public bool IsSystem { get; private set; }

    private readonly List<RolePermission> _rolePermissions = [];
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Role() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private Role(RoleId id, string name, bool isSystem, string? description = null) : base(id)
    {
        Name = name;
        Description = description;
        IsSystem = isSystem;
    }

    public static Result<Role> CreateSystemRole(string name, string? description = null)
    {
        var nameResult = ValidateName(name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        var descResult = ValidateDescription(description);
        if (descResult.IsFailure)
            return descResult.Error;

        return new Role(RoleId.New(), name, true, description);
    }

    public static Result<Role> Create(string name, string? description = null)
    {
        var nameResult = ValidateName(name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        var descResult = ValidateDescription(description);
        if (descResult.IsFailure)
            return descResult.Error;

        return new Role(RoleId.New(), name.Trim(), false, description);
    }

    public Result UpdateName(string name)
    {
        if (IsSystem)
            return RoleErrors.SystemRoleCannotBeModified;

        var nameResult = ValidateName(name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        Name = name.Trim();

        return Result.Success();
    }

    public Result UpdateDescription(string? description)
    {
        if (IsSystem)
            return RoleErrors.SystemRoleCannotBeModified;

        var descResult = ValidateDescription(description);
        if (descResult.IsFailure)
            return descResult.Error;

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

    private static Result ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return RoleErrors.NameEmpty;

        name = name.Trim();
        if (name.Length < 2)
            return RoleErrors.NameTooShort;
        if (name.Length > 100)
            return RoleErrors.NameTooLong;

        return Result.Success();
    }

    private static Result ValidateDescription(string? description)
    {
        if (!string.IsNullOrWhiteSpace(description) && description.Length > 500)
            return RoleErrors.DescriptionTooLong;

        return Result.Success();
    }
}