using Microsoft.AspNetCore.Authentication.JwtBearer;
using SharedKernel.Identity.Extensions;

namespace Identity.Api.Infrastructure.Authorization;

public static class IdentityAuthorizationPolicies
{
    public static IServiceCollection AddIdentityAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(SystemPermissionsCatalog.IdentityPermissions.UsersRead.Code, policy =>
            {
                policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
                policy.RequirePermission(SystemPermissionsCatalog.IdentityPermissions.UsersRead.Code);
            });

            options.AddPolicy(SystemPermissionsCatalog.IdentityPermissions.UsersCreate.Code, policy =>
            {
                policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
                policy.RequirePermission(SystemPermissionsCatalog.IdentityPermissions.UsersCreate.Code);
            });

            options.AddPolicy(SystemPermissionsCatalog.IdentityPermissions.UsersUpdate.Code, policy =>
            {
                policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
                policy.RequirePermission(SystemPermissionsCatalog.IdentityPermissions.UsersUpdate.Code);
            });

            options.AddPolicy(SystemPermissionsCatalog.IdentityPermissions.UsersDelete.Code, policy =>
            {
                policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
                policy.RequirePermission(SystemPermissionsCatalog.IdentityPermissions.UsersDelete.Code);
            });

            options.AddPolicy(SystemPermissionsCatalog.IdentityPermissions.UsersAssignRole.Code, policy =>
            {
                policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
                policy.RequirePermission(SystemPermissionsCatalog.IdentityPermissions.UsersAssignRole.Code);
            });

            options.AddPolicy(SystemPermissionsCatalog.IdentityPermissions.UsersRemoveRole.Code, policy =>
            {
                policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
                policy.RequirePermission(SystemPermissionsCatalog.IdentityPermissions.UsersRemoveRole.Code);
            });

            options.AddPolicy(SystemPermissionsCatalog.IdentityPermissions.RolesRead.Code, policy =>
            {
                policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
                policy.RequirePermission(SystemPermissionsCatalog.IdentityPermissions.RolesRead.Code);
            });

            options.AddPolicy(SystemPermissionsCatalog.IdentityPermissions.RolesCreate.Code, policy =>
            {
                policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
                policy.RequirePermission(SystemPermissionsCatalog.IdentityPermissions.RolesCreate.Code);
            });

            options.AddPolicy(SystemPermissionsCatalog.IdentityPermissions.RolesUpdate.Code, policy =>
            {
                policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
                policy.RequirePermission(SystemPermissionsCatalog.IdentityPermissions.RolesUpdate.Code);
            });

            options.AddPolicy(SystemPermissionsCatalog.IdentityPermissions.RolesDelete.Code, policy =>
            {
                policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
                policy.RequirePermission(SystemPermissionsCatalog.IdentityPermissions.RolesDelete.Code);
            });

            options.AddPolicy(SystemPermissionsCatalog.IdentityPermissions.RolesAssignPermission.Code, policy =>
            {
                policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
                policy.RequirePermission(SystemPermissionsCatalog.IdentityPermissions.RolesAssignPermission.Code);
            });

            options.AddPolicy(SystemPermissionsCatalog.IdentityPermissions.RolesRemovePermission.Code, policy =>
            {
                policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
                policy.RequirePermission(SystemPermissionsCatalog.IdentityPermissions.RolesRemovePermission.Code);
            });

            options.AddPolicy(SystemPermissionsCatalog.IdentityPermissions.PermissionsRead.Code, policy =>
            {
                policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
                policy.RequirePermission(SystemPermissionsCatalog.IdentityPermissions.PermissionsRead.Code);
            });
        });

        return services;
    }
}