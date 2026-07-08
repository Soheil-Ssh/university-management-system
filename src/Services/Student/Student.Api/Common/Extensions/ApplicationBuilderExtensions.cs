using Scalar.AspNetCore;
using SharedKernel.Persistence.Database;

namespace Student.Api.Common.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task<WebApplication> UseStudentPipeline(this WebApplication app)
    {
        await app.Services.ApplyMigrationsAsync<StudentDbContext>();

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

                            authorizationCodeFlow.AddQueryParameter("scope", "openid profile email offline_access student-api");
                            flows.AuthorizationCode = authorizationCodeFlow;
                        });
                })
                .AllowAnonymous();
        }

        app.UseHttpsRedirectionExceptHealthChecks();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseExceptionHandler();
        app.MapCarter();

        return app;
    }
}