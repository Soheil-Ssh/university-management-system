using Carter;
using Scalar.AspNetCore;
using SharedKernel.Api.Extensions;
using SharedKernel.Persistence.Database;

namespace Notification.Api.Common.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task<WebApplication> UseNotificationPipeline(this WebApplication app)
    {
        await app.Services.ApplyMigrationsAsync<NotificationDbContext>();

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

                            authorizationCodeFlow.AddQueryParameter("scope", "openid profile email offline_access notification-api");
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