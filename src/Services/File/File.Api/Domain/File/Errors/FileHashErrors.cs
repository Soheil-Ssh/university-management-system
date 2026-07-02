namespace File.Api.Domain.File.Errors;

public class FileHashErrors
{
    public static readonly Error Empty = new("FileHash.Empty", "File hash cannot be empty.", ErrorType.Validation);
    public static readonly Error InvalidFormat = 
        new("FileHash.InvalidFormat", "File hash must be a valid SHA-256 hexadecimal string (64 characters, 0-9 and A-F).", ErrorType.Validation);
}