using Identity.Api.Domain.User.Errors;
using Identity.Api.Domain.User.ValueObjects;

namespace Identity.Api.Domain.User;

public sealed class User : AggregateRoot<UserId>
{
    public string UserName { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<UserRole> _userRoles = [];
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private User() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private User(UserId id,
        string userName,
        Email email,
        string passwordHash,
        bool isActive) : base(id)
    {
        UserName = userName;
        Email = email;
        PasswordHash = passwordHash;
        IsActive = isActive;
    }

    public static Result<User> Create(string userName, Email email, string passwordHash, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return UserErrors.UserNameEmpty;

        userName = userName.Trim();

        if (userName.Length > 50)
            return UserErrors.UserNameTooLong;

        if (string.IsNullOrWhiteSpace(passwordHash))
            return UserErrors.PasswordHashEmpty;

        return new User(UserId.New(), userName, email, passwordHash, isActive);
    }

    public Result AssignRole(RoleId roleId)
    {
        if (_userRoles.Any(x => x.RoleId == roleId)) return UserErrors.RoleAlreadyExist;

        var roleResult = UserRole.Create(Id, roleId);
        if (roleResult.IsFailure)
            return roleResult.Error;

        _userRoles.Add(roleResult.Data);

        return Result.Success();
    }

    public Result RemoveRole(RoleId roleId)
    {
        var role = _userRoles.FirstOrDefault(x => x.RoleId == roleId);

        if (role is null)
            return UserErrors.RoleNotFound;

        _userRoles.Remove(role);

        return Result.Success();
    }

    public void ChangePassword(string hash)
    {
        PasswordHash = hash;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}