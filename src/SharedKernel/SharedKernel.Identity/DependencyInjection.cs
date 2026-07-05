using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Identity.Permissions;

namespace SharedKernel.Identity;

public static class DependencyInjection
{
    public static IServiceCollection AddUmsAuthorization(this IServiceCollection services,
        Action<AuthorizationOptions>? configure = null)
    {
        services.AddAuthorization(options =>
        {
            configure?.Invoke(options);
        });

        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        return services;
    }
}