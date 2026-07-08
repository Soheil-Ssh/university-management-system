namespace Identity.Api.Infrastructure.Persistence.Options;

public sealed record SuperAdminOptions
{
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Mobile { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}