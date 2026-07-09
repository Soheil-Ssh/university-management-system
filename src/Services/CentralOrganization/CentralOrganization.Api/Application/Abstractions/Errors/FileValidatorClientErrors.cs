namespace CentralOrganization.Api.Application.Abstractions.Errors;

public static class FileValidatorClientErrors
{
    public static readonly Error Unavailable = new("FileService.Unavailable",
        "File service is currently unavailable.", ErrorType.Unexpected);
}