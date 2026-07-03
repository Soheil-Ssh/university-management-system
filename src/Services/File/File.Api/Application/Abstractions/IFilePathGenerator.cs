namespace File.Api.Application.Abstractions;

public interface IFilePathGenerator
{
    string GetRelativePath(FileId id, FileName fileName);
}