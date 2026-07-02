namespace SharedKernel.Domain.Identifiers;

public sealed record UserId(Guid Value)
{
    public static UserId New() => new(Guid.NewGuid());
}