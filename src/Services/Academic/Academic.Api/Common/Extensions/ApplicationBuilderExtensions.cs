using Academic.Infrastructure.Persistence.Contexts;
using Scalar.AspNetCore;
using SharedKernel.Api.Extensions;
using SharedKernel.Persistence.Database;

namespace Academic.Api.Common.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task<WebApplication> UseAcademicPipeline(this WebApplication app)
    {
        await app.Services.ApplyMigrationsAsync<AcademicDbContext>();

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

                            authorizationCodeFlow.AddQueryParameter("scope", "openid profile email offline_access academic-api");
                            flows.AuthorizationCode = authorizationCodeFlow;
                        });
                })
                .AllowAnonymous();
        }

        app.UseHttpsRedirectionExceptPaths();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseExceptionHandler();
        app.MapCarter();

        return app;
    }
}