namespace Identity.Api.Common.Authorization.Permissions;

public sealed record PermissionDefinition(string Name,
    string Code,
    string Category);