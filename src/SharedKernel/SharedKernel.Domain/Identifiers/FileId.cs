namespace SharedKernel.Domain.Identifiers;

public sealed record FileId(Guid Value)
{
    public static FileId New() => new(Guid.NewGuid());
}