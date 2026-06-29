namespace Identity.Api.Domain.Role;

public sealed class Role : AggregateRoot<RoleId>
{
    public string Name { get; private set; }
    public string DisplayName { get; private set; }
    public string? Description { get; private set; }
    public bool IsSystem { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<RolePermission> _rolePermissions = [];
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Role() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private Role(
        RoleId id,
        string name,
        string displayName,
        bool isSystem,
        string? description = null)
        : base(id)
    {
        Name = name;
        DisplayName = displayName;
        Description = description;
        IsSystem = isSystem;
        IsActive = true;
    }

    public static Result<Role> CreateSystemRole(string name, string displayName, string? description = null)
    {
        var nameResult = ValidateName(name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        var displayNameResult = ValidateDisplayName(displayName);
        if (displayNameResult.IsFailure)
            return displayNameResult.Error;

        var descResult = ValidateDescription(description);
        if (descResult.IsFailure)
            return descResult.Error;

        return new Role(RoleId.New(), name.Trim(), displayName.Trim(), true, description?.Trim());
    }

    public static Result<Role> Create(string name, string displayName, string? description = null)
    {
        var nameResult = ValidateName(name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        var displayNameResult = ValidateDisplayName(displayName);
        if (displayNameResult.IsFailure)
            return displayNameResult.Error;

        var descResult = ValidateDescription(description);
        if (descResult.IsFailure)
            return descResult.Error;

        return new Role(RoleId.New(), name.Trim(), displayName.Trim(), false, description?.Trim());
    }

    public Result UpdateName(string name)
    {
        if (IsSystem)
            return RoleErrors.SystemRoleNameCannotBeChanged;

        var nameResult = ValidateName(name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        Name = name.Trim();

        return Result.Success();
    }

    public Result UpdateDisplayName(string displayName)
    {
        var displayNameResult = ValidateDisplayName(displayName);
        if (displayNameResult.IsFailure)
            return displayNameResult.Error;

        DisplayName = displayName.Trim();

        return Result.Success();
    }

    public Result UpdateDescription(string? description)
    {
        var descResult = ValidateDescription(description);
        if (descResult.IsFailure)
            return descResult.Error;

        Description = description?.Trim();

        return Result.Success();
    }

    public Result AddPermission(PermissionId permissionId)
    {
        // TODO Check current role is system or not because system role permissions cannot be modified

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
        // TODO Check current role is system or not because system role permissions cannot be modified

        var permission = _rolePermissions
            .FirstOrDefault(x => x.PermissionId == permissionId);

        if (permission is null)
            return RoleErrors.PermissionNotFound;

        _rolePermissions.Remove(permission);

        return Result.Success();
    }

    public Result Activate()
    {
        if (IsActive || IsSystem)
            return Result.Success();

        IsActive = true;

        return Result.Success();
    }

    public Result Deactivate()
    {
        if (IsSystem)
            return RoleErrors.SystemRoleCannotBeDeactivated;

        if (!IsActive)
            return Result.Success();

        IsActive = false;

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

    private static Result ValidateDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            return RoleErrors.DisplayNameEmpty;

        displayName = displayName.Trim();
        if (displayName.Length < 2)
            return RoleErrors.DisplayNameTooShort;
        if (displayName.Length > 150)
            return RoleErrors.DisplayNameTooLong;

        return Result.Success();
    }

    private static Result ValidateDescription(string? description)
    {
        if (!string.IsNullOrWhiteSpace(description) && description.Length > 500)
            return RoleErrors.DescriptionTooLong;

        return Result.Success();
    }
}