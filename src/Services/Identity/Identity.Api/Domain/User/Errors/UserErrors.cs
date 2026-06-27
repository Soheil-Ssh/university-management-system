namespace Identity.Api.Domain.User.Errors;

public static class UserErrors
{
    public static readonly Error UserNameEmpty = new("User.UserName.Empty", "Username cannot be empty.", ErrorType.Validation);
    public static readonly Error UserNameTooLong = new("User.UserName.TooLong", "Username is too long.", ErrorType.Validation);
    public static readonly Error PasswordHashEmpty = new("User.PasswordHash.Empty", "Password hash cannot be empty.", ErrorType.Validation);
    public static readonly Error RoleAlreadyExist = new("User.Role.AlreadyExist", "Role already exists for this user.", ErrorType.Validation);
    public static readonly Error RoleNotFound = new("User.Role.NotFound", "Role not found for this user.", ErrorType.Validation);
}