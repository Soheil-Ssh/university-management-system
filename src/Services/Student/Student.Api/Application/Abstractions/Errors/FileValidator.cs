namespace Student.Api.Application.Abstractions.Errors;

public static class FileValidator
{
    public static readonly Error Unavailable = new ("FileService.Unavailable",
        "File service is currently unavailable.", ErrorType.Unexpected);
}