using System.Security.Cryptography;

namespace Identity.Api.Domain.User;

public sealed class User : AggregateRoot<UserId>
{
    public string UserName { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string SecurityStamp { get; private set; }
    public bool MustChangePassword { get; private set; }
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
        string securityStamp,
        bool isActive) : base(id)
    {
        UserName = userName;
        Email = email;
        PasswordHash = passwordHash;
        SecurityStamp = securityStamp;
        IsActive = isActive;
        MustChangePassword = true;
    }

    public static Result<User> Create(string userName, Email email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return UserErrors.UserNameEmpty;

        userName = userName.Trim();

        if (userName.Length > 50)
            return UserErrors.UserNameTooLong;

        passwordHash = passwordHash.Trim();

        if (string.IsNullOrWhiteSpace(passwordHash))
            return UserErrors.PasswordHashEmpty;

        string securityStamp = GenerateSecurityStamp();

        return new User(UserId.New(), userName, email, passwordHash, securityStamp, true);
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

    public Result ChangePassword(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            return UserErrors.PasswordHashEmpty;

        PasswordHash = passwordHash;
        SecurityStamp = GenerateSecurityStamp();
        MustChangePassword = false;

        return Result.Success();
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private static string GenerateSecurityStamp()
        => Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
}