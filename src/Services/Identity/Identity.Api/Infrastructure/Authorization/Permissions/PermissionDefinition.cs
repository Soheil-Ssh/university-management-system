namespace Identity.Api.Infrastructure.Authorization.Permissions;

public sealed record PermissionDefinition(string Name, string DisplayName, string Code, string Category);