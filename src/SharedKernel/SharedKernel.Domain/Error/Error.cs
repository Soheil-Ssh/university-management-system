namespace SharedKernel.Domain.Error;

public sealed record Error(string Code, string? Description = null, ErrorType Type = ErrorType.None)
{
    public static readonly Error None = new Error(string.Empty);

    public override string ToString() =>
        string.IsNullOrEmpty(Description) ? Code : $"{Code}: {Description}";
}