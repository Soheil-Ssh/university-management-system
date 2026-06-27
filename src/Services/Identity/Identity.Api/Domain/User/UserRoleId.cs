namespace Identity.Api.Domain.User;

public sealed record UserRoleId(Guid Value)
{
    public static UserRoleId New() => new(Guid.NewGuid());
}