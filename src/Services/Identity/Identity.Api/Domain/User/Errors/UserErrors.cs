namespace Identity.Api.Domain.User.Errors;

public static class UserErrors
{
    // General errors
    // User name errors
    public static readonly Error UserNameEmpty = new("User.UserName.Empty", "Username cannot be empty.", ErrorType.Validation);
    public static readonly Error UserNameTooLong = new("User.UserName.TooLong", "Username is too long.", ErrorType.Validation);
    public static readonly Error UsernameAlreadyExists = new("User.UserName.AlreadyExists", "Username is already taken.", ErrorType.Validation);

    // Password hash errors
    public static readonly Error PasswordHashEmpty = new("User.PasswordHash.Empty", "Password hash cannot be empty.", ErrorType.Validation);

    // Email errors
    public static readonly Error EmailAlreadyExists = new("User.Email.AlreadyExists", "Email is already registered.", ErrorType.Validation);

    // Role errors
    public static readonly Error RoleAlreadyExist = new("User.Role.AlreadyExist", "Role already exists for this user.", ErrorType.Validation);
    public static readonly Error RoleNotFound = new("User.Role.NotFound", "Role not found for this user.", ErrorType.Validation);
}