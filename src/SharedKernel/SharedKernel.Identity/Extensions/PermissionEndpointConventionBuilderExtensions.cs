using Microsoft.AspNetCore.Builder;
using SharedKernel.Identity.Permissions;

namespace SharedKernel.Identity.Extensions;

public static class PermissionEndpointConventionBuilderExtensions
{
    public static TBuilder RequirePermission<TBuilder>(
        this TBuilder builder,
        string permission)
        where TBuilder : IEndpointConventionBuilder
    {
        builder.RequireAuthorization(PermissionPolicyNames.For(permission));
        return builder;
    }
}