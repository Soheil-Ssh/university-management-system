namespace File.Api.Domain.File.Errors;

public static class FileNameErrors
{
    public static readonly Error Empty = new("FileName.Empty", "File name cannot be empty.", ErrorType.Validation);
    public static readonly Error TooLong = new("FileName.TooLong", "File name cannot exceed 255 characters.", ErrorType.Validation);
    public static readonly Error ContainsInvalidChars = 
        new("FileName.InvalidChars", "File name contains invalid characters (e.g., \\, /, :, *, ?, \", <, >, |).", ErrorType.Validation);
}