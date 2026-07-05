namespace SharedKernel.Identity.Permissions;

public static class PermissionPolicyNames
{
    public const string Prefix = "Permission:";

    public static string For(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
            throw new ArgumentException("Permission is required.", nameof(permission));

        return $"{Prefix}{permission}";
    }

    public static bool TryParse(string policyName, out string permission)
    {
        permission = string.Empty;

        if (string.IsNullOrWhiteSpace(policyName))
            return false;

        if (!policyName.StartsWith(Prefix, StringComparison.Ordinal))
            return false;

        permission = policyName[Prefix.Length..];

        return !string.IsNullOrWhiteSpace(permission);
    }
}