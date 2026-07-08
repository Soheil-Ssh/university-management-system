using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using SharedKernel.Identity.Authorization.Claims;
using System.Security.Claims;

namespace Identity.Api.Infrastructure.IdentityServer;

public class IdentityServerProfileService(IdentityDbContext dbContext) : IProfileService
{
    public async Task GetProfileDataAsync(ProfileDataRequestContext context, CancellationToken ct)
    {
        string? subject = context.Subject.FindFirstValue("sub");

        if (!Guid.TryParse(subject, out Guid userId))
            return;

        var user = await dbContext.Users
            .AsNoTracking()
            .Where(x => x.Id == new UserId(userId))
            .Select(x => new
            {
                UserId = x.Id.Value,
                x.UserName,
                Email = x.Email.Value,
                FirstName = x.FirstName.Value,
                LastName = x.LastName.Value,
                Mobile = x.Mobile.Value,
                x.SecurityStamp,
                x.MustChangePassword,
                x.IsActive,
                x.UserRoles
            })
            .FirstOrDefaultAsync(ct);

        if (user is null || !user.IsActive)
            return;

        var roleIds = user.UserRoles.Select(x => x.RoleId).Distinct().ToList();

        var roles = await dbContext.Roles
            .AsNoTracking()
            .Include(x => x.RolePermissions)
            .Where(x => roleIds.Contains(x.Id) && x.IsActive)
            .ToListAsync(ct);

        var permissionIds = roles
            .SelectMany(x => x.RolePermissions)
            .Select(x => x.PermissionId)
            .Distinct()
            .ToList();

        var permissions = await dbContext.Permissions
            .AsNoTracking()
            .Where(x => permissionIds.Contains(x.Id))
            .Select(x => x.Code)
            .Distinct()
            .ToListAsync(ct);

        var claims = new List<Claim>
        {
            new(UmsClaimTypes.Subject, user.UserId.ToString()),
            new(UmsClaimTypes.UserName, user.UserName),
            new(UmsClaimTypes.Email, user.Email),
            new(UmsClaimTypes.GivenName, user.FirstName),
            new(UmsClaimTypes.FamilyName, user.LastName),
            new(UmsClaimTypes.FullName, $"{user.FirstName} {user.LastName}"),
            new(UmsClaimTypes.SecurityStamp, user.SecurityStamp),
            new(UmsClaimTypes.MustChangePassword, user.MustChangePassword.ToString().ToLowerInvariant())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name));
            claims.Add(new Claim(UmsClaimTypes.Role, role.Name));
        }

        claims.AddRange(permissions.Select(permission => new Claim(UmsClaimTypes.Permission, permission)));

        context.IssuedClaims.AddRange(claims);
    }

    public async Task IsActiveAsync(IsActiveContext context, CancellationToken ct)
    {
        string? subject = context.Subject.FindFirstValue("sub");

        if (!Guid.TryParse(subject, out Guid userIdValue))
        {
            context.IsActive = false;
            return;
        }

        var userId = new UserId(userIdValue);

        context.IsActive = await dbContext.Users
            .AsNoTracking()
            .AnyAsync(x => x.Id == userId && x.IsActive, ct);
    }
}