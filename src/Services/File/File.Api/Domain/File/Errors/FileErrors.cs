namespace File.Api.Domain.File.Errors;

using SharedKernel.Domain.Error;

public static class FileErrors
{
    // General errors
    public static readonly Error NotFound = new("File.NotFound", "File not found.", ErrorType.NotFound);
    public static readonly Error AlreadyExists = new("File.AlreadyExists", "File already exists.", ErrorType.Conflict);

    // Mime type errors
    public static readonly Error MimeTypeEmpty = new("File.MimeType.Empty", "Mime type cannot be empty.", ErrorType.Validation);
    public static readonly Error MimeTypeTooLong = new("File.MimeType.TooLong", "MimeMime type is too long.", ErrorType.Validation);

    // Size errors
    public static readonly Error SizeInvalid = new("File.Size.Invalid", "File size must be greater than zero.", ErrorType.Validation);

    // Status transition errors
    public static readonly Error AlreadyAttached = new("File.Status.AlreadyAttached", "File is already attached to an entity and cannot be attached again.", ErrorType.Conflict);
    public static readonly Error NotAttached = new("File.Status.NotAttached", "File is not attached.", ErrorType.Conflict);
    public static readonly Error AlreadyDeleted = new("File.Status.AlreadyDeleted", "File is already deleted.", ErrorType.Conflict);
    public static readonly Error CannotAttachDeleted = new("File.Status.CannotAttachDeleted", "Cannot attach a deleted file.", ErrorType.Conflict);
    public static readonly Error InvalidStatusTransition = new("File.InvalidStatusTransition", "Invalid status transition for this operation.", ErrorType.Conflict);
}
