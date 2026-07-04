using File.Api.Domain.File.Errors;

namespace File.Api.Domain.File;

public sealed class File : AggregateRoot<FileId>
{
    public FileName FileName { get; private set; }
    public string MimeType { get; private set; }
    public FileHash? Hash { get; private set; }
    public long Size { get; private set; }
    public FileStatus Status { get; private set; }
    public UserId? UploadedBy { get; private set; }
    public string StoragePath { get; private set; }
    public DateTime? AttachedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
    private File() { }
#pragma warning restore CS8618

    private File(FileId id,
        FileName fileName,
        string mimeType,
        long size,
        UserId? uploadedBy,
        FileHash? hash = null) : base(id)
    {
        FileName = fileName;
        MimeType = mimeType;
        Size = size;
        UploadedBy = uploadedBy;
        Hash = hash;
        StoragePath = string.Empty;
        Status = FileStatus.Temporary;
    }

    public static Result<File> Create(string fileName, string mimeType, long size, UserId? uploadedBy)
    {
        var fileNameResult = FileName.Create(fileName);
        if (fileNameResult.IsFailure)
            return fileNameResult.Error;

        if (string.IsNullOrWhiteSpace(mimeType))
            return FileErrors.MimeTypeEmpty;

        mimeType = mimeType.Trim();
        if (mimeType.Length > 100)
            return FileErrors.MimeTypeEmpty;

        if (size <= 0)
            return FileErrors.SizeInvalid;

        return new File(FileId.New(), fileNameResult.Data, mimeType, size, uploadedBy);
    }

    public Result SetStoragePath(string storagePath)
    {
        if (string.IsNullOrWhiteSpace(storagePath))
            return FileErrors.StoragePathEmpty;

        storagePath = storagePath.Trim().Replace('\\', '/');

        if (storagePath.StartsWith('/') || storagePath.Contains("../"))
            return FileErrors.StoragePathInvalid;

        if (storagePath.Length > 500)
            return FileErrors.StoragePathInvalid;

        StoragePath = storagePath;

        return Result.Success();
    }

    public Result Attach()
    {
        if (Status == FileStatus.Attached)
            return FileErrors.AlreadyAttached;

        if (Status == FileStatus.Deleted)
            return FileErrors.CannotAttachDeleted;

        if (Status != FileStatus.Temporary)
            return FileErrors.InvalidStatusTransition;

        Status = FileStatus.Attached;
        AttachedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public Result Detach()
    {
        if (Status != FileStatus.Attached)
            return FileErrors.NotAttached;

        Status = FileStatus.Temporary;
        AttachedAt = null;

        return Result.Success();
    }

    public Result Delete()
    {
        if (Status == FileStatus.Deleted)
            return FileErrors.AlreadyDeleted;

        Status = FileStatus.Deleted;
        DeletedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public Result SetHash(string fileHash)
    {
        var hashResult = FileHash.Create(fileHash);
        if (hashResult.IsFailure)
            return hashResult.Error;

        Hash = hashResult.Data;
        return Result.Success();
    }

    public Result UpdateNameWithoutExtension(string newNameWithoutExtension)
    {
        if (Status == FileStatus.Deleted)
            return FileErrors.CannotRenameDeleted;

        var fileNameResult = FileName.CreateFromNameWithoutExtension(
            newNameWithoutExtension,
            FileName.Extension);

        if (fileNameResult.IsFailure)
            return fileNameResult.Error;

        var newFileName = fileNameResult.Data;

        if (string.Equals(FileName.Value, newFileName.Value,
                StringComparison.OrdinalIgnoreCase))
        {
            return FileErrors.FileNameNotChanged;
        }

        FileName = newFileName;

        return Result.Success();
    }
}
