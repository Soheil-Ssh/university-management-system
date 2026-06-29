namespace Identity.Api.Common.Authorization.Roles;

public sealed record RoleDefinition(
    string Name,
    string DisplayName,
    string? Description = null);