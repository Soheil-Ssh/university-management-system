namespace CentralOrganization.Api.Application.Abstractions.Identity.Errors;

public static class IdentityUserClientErrors
{
    public static readonly Error ServiceUnavailable = new("Identity.ServiceUnavailable", "Identity service is currently unavailable.", ErrorType.Unexpected);
    public static readonly Error DeadlineExceeded = new("Identity.DeadlineExceeded", "Identity service did not respond in time.", ErrorType.Unexpected);
    public static readonly Error InvalidResponse = new("Identity.InvalidResponse", "Identity service returned an invalid response.", ErrorType.Unexpected);
}