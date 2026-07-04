using File.Api.Infrastructure.Persistence.Options;
using Microsoft.Extensions.Options;

namespace File.Api.Infrastructure.Storage;

public class LocalFileStorage(IOptions<FileStorageOptions> options) : IFileStorage
{
    public async Task SaveAsync(Stream stream, string relativePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);

        cancellationToken.ThrowIfCancellationRequested();

        string path = ResolvePath(relativePath);

        var directory = Path.GetDirectoryName(path);

        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        await using var fileStream = new FileStream(
            path,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            useAsync: true);

        if (stream.CanSeek)
            stream.Position = 0;

        await stream.CopyToAsync(fileStream, cancellationToken);
    }

    public Task<Stream> OpenReadAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);

        cancellationToken.ThrowIfCancellationRequested();

        string path = ResolvePath(relativePath);

        if (!System.IO.File.Exists(path))
            throw new FileNotFoundException("The file does not exist.", path);

        Stream stream = new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 81920,
            useAsync: true);

        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);

        cancellationToken.ThrowIfCancellationRequested();

        string path = ResolvePath(relativePath);

        if (!System.IO.File.Exists(path))
            return Task.CompletedTask;

        System.IO.File.Delete(path);

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(System.IO.File.Exists(ResolvePath(relativePath)));
    }

    private string ResolvePath(string relativePath)
    {
        if (Path.IsPathRooted(relativePath))
            throw new InvalidOperationException("Invalid file path.");

        string root = Path.GetFullPath(options.Value.RootPath);

        string fullPath = Path.GetFullPath(Path.Combine(root, relativePath));

        string relativeToRoot = Path.GetRelativePath(root, fullPath);

        bool isOutsideRoot = relativeToRoot == ".." ||
                             relativeToRoot.StartsWith($"..{Path.DirectorySeparatorChar}") ||
                             relativeToRoot.StartsWith($"..{Path.AltDirectorySeparatorChar}") ||
                             Path.IsPathRooted(relativeToRoot);

        if (isOutsideRoot)
            throw new InvalidOperationException("Invalid file path.");

        return fullPath;
    }
}