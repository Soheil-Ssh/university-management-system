namespace SharedKernel.Identity.Authorization.Options;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Authority { get; init; } = string.Empty;
    public string? MetadataAddress { get; init; }
    public bool RequireHttpsMetadata { get; init; } = true;

    public bool ValidateIssuer { get; init; } = true;
    public string? ValidIssuer { get; init; }

    public bool ValidateAudience { get; init; } = true;
    public string Audience { get; init; } = string.Empty;

    public bool ValidateLifetime { get; init; } = true;
    public int ClockSkewSeconds { get; init; } = 60;

    public bool MapInboundClaims { get; init; } = false;
    public string NameClaimType { get; init; } = "username";
    public string RoleClaimType { get; init; } = "role";

    public bool IncludeErrorDetails { get; init; } = true;
    public bool SaveToken { get; init; } = false;

    public bool BackchannelIgnoreCertificateErrors { get; init; } = false;
}