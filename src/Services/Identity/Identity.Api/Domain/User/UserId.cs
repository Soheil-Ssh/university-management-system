namespace Identity.Api.Domain.User;

public sealed record UserId(Guid Value)
{
    public static UserId New() => new(Guid.NewGuid());
}