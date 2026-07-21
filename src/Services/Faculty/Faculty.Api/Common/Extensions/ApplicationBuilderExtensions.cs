using Faculty.Api.Infrastructure.Grpc;
using Scalar.AspNetCore;
using SharedKernel.Persistence.Database;

namespace Faculty.Api.Common.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task<WebApplication> UseFacultyPipeline(this WebApplication app)
    {
        await app.Services.ApplyMigrationsAsync<FacultyDbContext>();

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

                            authorizationCodeFlow.AddQueryParameter("scope", "openid profile email offline_access faculty-api");
                            flows.AuthorizationCode = authorizationCodeFlow;
                        });
                })
                .AllowAnonymous();
        }

        app.UseHttpsRedirectionExceptPaths(configure =>
        {
            configure.ExcludedPathPrefixes.Add("/sharedkernel.contracts.grpc.faculty.v1.DepartmentValidationService");
        });
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseExceptionHandler();
        app.MapGrpcService<DepartmentValidationGrpcService>();
        app.MapCarter();

        return app;
    }
}