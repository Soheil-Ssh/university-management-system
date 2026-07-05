using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Identity.Api.Infrastructure.IdentityServer;

public static class IdentityServerConfig
{
    public static IEnumerable<IdentityResource> IdentityResources =>
    [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResources.Email(),
    ];

    public static IEnumerable<ApiScope> ApiScopes =>
    [

        new("identity-api", "Identity API"),
        new("student-api", "Student API"),
        new("file-api", "File API")
    ];

    public static IEnumerable<ApiResource> ApiResources =>
    [
        new("identity-api", "Identity API") { Scopes = { "identity-api" } },
        new("student-api", "Student API") { Scopes = { "student-api" } },
        new("file-api", "File API") { Scopes = { "file-api" } }
    ];

    public static IEnumerable<Client> Clients =>
    [
        new Client
        {
            ClientId = "ums-scalar",
            ClientName = "UMS Scalar",

            AllowedGrantTypes = GrantTypes.Code,
            RequirePkce = true,
            RequireClientSecret = false,

            RedirectUris =
            {
                "https://localhost:5051/scalar/",
                "https://localhost:5051/scalar"
            },

            PostLogoutRedirectUris =
            {
                "https://localhost:5051/scalar/",
                "https://localhost:5051/scalar"
            },

            AllowedCorsOrigins =
            {
                "https://localhost:5051"
            },

            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.Email,
                IdentityServerConstants.StandardScopes.OfflineAccess,
                "identity-api",
            },

            RequireConsent = false,
            AllowOfflineAccess = true,
            AccessTokenLifetime = 3600,
            RefreshTokenUsage = TokenUsage.ReUse,
            RefreshTokenExpiration = TokenExpiration.Sliding,
            SlidingRefreshTokenLifetime = 60 * 60 * 24 * 15,
            AbsoluteRefreshTokenLifetime = 60 * 60 * 24 * 30
        }
    ];
}