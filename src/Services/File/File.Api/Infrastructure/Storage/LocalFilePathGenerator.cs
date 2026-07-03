namespace File.Api.Infrastructure.Storage;

public class LocalFilePathGenerator : IFilePathGenerator
{
    public string GetRelativePath(FileId id, FileName fileName)
        => $"{DateTime.UtcNow:yyyy/MM}/{id.Value}{fileName.Extension}";
}