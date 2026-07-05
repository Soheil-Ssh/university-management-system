using Scalar.AspNetCore;
using SharedKernel.Persistence.Database;

namespace Identity.Api.Common.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task<WebApplication> UseIdentityPipeline(this WebApplication app)
    {
        await app.Services.ApplyMigrationsAsync<IdentityDbContext>();
        await app.Services.SeedDatabaseAsync<IdentityDbContext>();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference("/scalar", options =>
                {
                    options
                        .AddPreferredSecuritySchemes("OAuth2")
                        .AddOAuth2Flows("OAuth2", flows =>
                        {
                            var authorizationCodeFlow = new AuthorizationCodeFlow
                            {
                                ClientId = "ums-scalar",
                                AuthorizationUrl = "https://localhost:5051/connect/authorize",
                                TokenUrl = "https://localhost:5051/connect/token",
                                Pkce = Pkce.Sha256
                            };

                            authorizationCodeFlow.AddQueryParameter("scope",
                                "openid profile email offline_access identity-api");

                            flows.AuthorizationCode = authorizationCodeFlow;
                        });
                })
                .AllowAnonymous();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseIdentityServer();
        app.UseAuthorization();
        app.UseExceptionHandler();
        app.MapCarter();
        app.MapRazorPages();

        return app;
    }
}