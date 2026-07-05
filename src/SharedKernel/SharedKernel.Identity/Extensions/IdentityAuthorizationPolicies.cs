using Microsoft.AspNetCore.Authorization;
using SharedKernel.Identity.Authorization.Claims;

namespace SharedKernel.Identity.Extensions;

public static class IdentityAuthorizationPolicies
{
    public static AuthorizationPolicyBuilder RequirePermission(this AuthorizationPolicyBuilder builder, string permissionCode)
        => builder.RequireClaim(UmsClaimTypes.Permission, permissionCode);
}