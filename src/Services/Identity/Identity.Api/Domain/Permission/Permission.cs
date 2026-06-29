namespace Identity.Api.Domain.Permission;

public sealed class Permission : AggregateRoot<PermissionId>
{
    public string Name { get; private set; }
    public string DisplayName { get; private set; }
    public string Code { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Permission() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private Permission(PermissionId id, string name, string displayName, string code) : base(id)
    {
        Name = name;
        DisplayName = displayName;
        Code = code;
    }

    public static Result<Permission> Create(string name, string displayName, string code)
    {
        if (string.IsNullOrWhiteSpace(name))
            return PermissionErrors.NameEmpty;

        name = name.Trim();
        if (name.Length > 150)
            return PermissionErrors.NameTooLong;

        if (string.IsNullOrWhiteSpace(displayName))
            return PermissionErrors.DisplayNameEmpty;

        displayName = displayName.Trim();
        if (displayName.Length > 150)
            return PermissionErrors.DisplayNameTooLong;

        if (string.IsNullOrWhiteSpace(code))
            return PermissionErrors.CodeEmpty;

        code = code.Trim();
        if (code.Length > 150)
            return PermissionErrors.CodeTooLong;

        return new Permission(PermissionId.New(), name, displayName, code);
    }

    public Result SyncWithSystemDefinition(string name, string displayName)
    {
        Name = name;
        DisplayName = displayName;

        return Result.Success();
    }
}