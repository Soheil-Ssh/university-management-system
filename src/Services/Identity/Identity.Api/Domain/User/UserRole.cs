namespace Identity.Api.Domain.User;

public class UserRole : Entity<UserRoleId>
{
    public UserId UserId { get; private set; }
    public RoleId RoleId { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private UserRole() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private UserRole(UserRoleId id, UserId userId, RoleId roleId)
        : base(id)
    {
        UserId = userId;
        RoleId = roleId;
    }

    public static Result<UserRole> Create(UserId userId, RoleId roleId)
        => new UserRole(UserRoleId.New(), userId, roleId);
}