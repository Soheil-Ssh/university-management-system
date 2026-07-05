using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Identity.Authorization.Options;
using System.IdentityModel.Tokens.Jwt;

namespace SharedKernel.Identity.Extensions;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddUmsJwtAuthentication(this IServiceCollection services,
        IConfiguration configuration,
        bool useJwtBearerAsDefaultScheme = true)
    {
        var jwtOptions = configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>()
            ?? throw new InvalidOperationException("Jwt configuration is missing.");

        if (string.IsNullOrWhiteSpace(jwtOptions.Authority))
            throw new InvalidOperationException("Jwt:Authority is required.");

        if (jwtOptions.ValidateAudience && string.IsNullOrWhiteSpace(jwtOptions.Audience))
            throw new InvalidOperationException("Jwt:Audience is required when ValidateAudience is true.");

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        AuthenticationBuilder authenticationBuilder;

        if (useJwtBearerAsDefaultScheme)
        {
            authenticationBuilder = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            });
        }
        else
        {
            authenticationBuilder = services.AddAuthentication();
        }

        authenticationBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.Authority = jwtOptions.Authority;

            if (!string.IsNullOrWhiteSpace(jwtOptions.MetadataAddress))
                options.MetadataAddress = jwtOptions.MetadataAddress;

            Console.WriteLine($"---------------- {jwtOptions.MetadataAddress} ------------------");

            options.RequireHttpsMetadata = jwtOptions.RequireHttpsMetadata;
            options.MapInboundClaims = jwtOptions.MapInboundClaims;
            options.IncludeErrorDetails = jwtOptions.IncludeErrorDetails;
            options.SaveToken = jwtOptions.SaveToken;

            if (jwtOptions.BackchannelIgnoreCertificateErrors)
            {
                options.BackchannelHttpHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
            }

            options.TokenValidationParameters = new TokenValidationParameters
            {
                
                ValidateIssuer = jwtOptions.ValidateIssuer,
                ValidIssuer = jwtOptions.ValidIssuer ?? jwtOptions.Authority,

                ValidateAudience = jwtOptions.ValidateAudience,
                ValidAudience = jwtOptions.Audience,

                ValidateLifetime = jwtOptions.ValidateLifetime,
                ClockSkew = TimeSpan.FromSeconds(jwtOptions.ClockSkewSeconds),

                ValidateIssuerSigningKey = true,
                RequireSignedTokens = true,

                NameClaimType = jwtOptions.NameClaimType,
                RoleClaimType = jwtOptions.RoleClaimType
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var logger = GetLogger(context.HttpContext);

                    logger.LogDebug(
                        "JWT message received. HasAuthorizationHeader: {HasAuthorizationHeader}, Path: {Path}",
                        context.Request.Headers.ContainsKey("Authorization"),
                        context.Request.Path.Value);

                    return Task.CompletedTask;
                },

                OnTokenValidated = context =>
                {
                    var logger = GetLogger(context.HttpContext);

                    string? subject = context.Principal?.FindFirst("sub")?.Value;
                    string? username = context.Principal?.FindFirst("username")?.Value;
                    string? clientId = context.Principal?.FindFirst("client_id")?.Value;

                    var roles = context.Principal?
                        .FindAll(jwtOptions.RoleClaimType)
                        .Select(x => x.Value)
                        .ToArray() ?? [];

                    var permissions = context.Principal?
                        .FindAll("permission")
                        .Select(x => x.Value)
                        .ToArray() ?? [];

                    logger.LogInformation(
                        "JWT token validated. Subject: {Subject}, Username: {Username}, ClientId: {ClientId}, RolesCount: {RolesCount}, PermissionsCount: {PermissionsCount}",
                        subject,
                        username,
                        clientId,
                        roles.Length,
                        permissions.Length);

                    logger.LogDebug(
                        "JWT authorization claims. Roles: {@Roles}, Permissions: {@Permissions}",
                        roles,
                        permissions);

                    return Task.CompletedTask;
                },

                OnAuthenticationFailed = context =>
                {
                    var logger = GetLogger(context.HttpContext);

                    logger.LogWarning(
                        context.Exception,
                        "JWT authentication failed. Path: {Path}, ErrorMessage: {ErrorMessage}",
                        context.HttpContext.Request.Path.Value,
                        context.Exception.Message);

                    return Task.CompletedTask;
                },

                OnChallenge = context =>
                {
                    var logger = GetLogger(context.HttpContext);

                    logger.LogWarning(
                        "JWT challenge triggered. Path: {Path}, Error: {Error}, ErrorDescription: {ErrorDescription}",
                        context.HttpContext.Request.Path.Value,
                        context.Error,
                        context.ErrorDescription);

                    return Task.CompletedTask;
                },

                OnForbidden = context =>
                {
                    var logger = GetLogger(context.HttpContext);

                    logger.LogWarning(
                        "JWT authorization forbidden. Path: {Path}, Subject: {Subject}, Username: {Username}",
                        context.HttpContext.Request.Path.Value,
                        context.HttpContext.User.FindFirst("sub")?.Value,
                        context.HttpContext.User.FindFirst("username")?.Value);

                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }

    private static ILogger GetLogger(HttpContext httpContext)
    {
        var loggerFactory = httpContext.RequestServices
            .GetRequiredService<ILoggerFactory>();

        return loggerFactory.CreateLogger("Ums.JwtBearer");
    }
}