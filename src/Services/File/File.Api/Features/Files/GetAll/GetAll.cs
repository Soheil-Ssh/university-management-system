namespace File.Api.Features.Files.GetAll;

public static class GetAll
{
    public sealed record GetAllFilesRequest(
        string? FileName,
        string? Extension,
        string? MimeType,
        FileStatus? Status,
        long? MinSize,
        long? MaxSize,
        DateTime? FromCreateAt,
        DateTime? ToCreateAt,
        DateTime? FromUpdateAt,
        DateTime? ToUpdateAt,
        int Page = 1,
        int PageSize = 20);

    public sealed record Response(
        Guid Id,
        string FileName,
        string NameWithoutExtension,
        string Extension,
        string MimeType,
        long Size,
        FileStatus Status,
        DateTime CreateAt,
        DateTime UpdateAt);

    public sealed record Query(
        string? FileName,
        string? Extension,
        string? MimeType,
        FileStatus? Status,
        long? MinSize,
        long? MaxSize,
        DateTime? FromCreateAt,
        DateTime? ToCreateAt,
        DateTime? FromUpdateAt,
        DateTime? ToUpdateAt,
        int Page = 1,
        int PageSize = 20)
        : IQuery<Result<PagedResult<Response>>>;

    public class Handler(FileDbContext context) : IQueryHandler<Query, Result<PagedResult<Response>>>
    {
        public async Task<Result<PagedResult<Response>>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var query = context.Files.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.FileName))
                query = query.Where(f => f.FileName.Value.Contains(request.FileName.Trim()));

            if (!string.IsNullOrWhiteSpace(request.Extension))
            {
                var extension = request.Extension.Trim();

                if (!extension.StartsWith('.'))
                    extension = $".{extension}";

                query = query.Where(f =>
                    f.FileName.Value.EndsWith(extension));
            }

            if (!string.IsNullOrWhiteSpace(request.MimeType))
                query = query.Where(f => f.MimeType.Contains(request.MimeType.Trim()));

            query = request.Status.HasValue ?
                query.Where(f => f.Status == request.Status.Value) :
                query.Where(f => f.Status != FileStatus.Deleted);

            if (request.MinSize.HasValue)
                query = query.Where(f => f.Size >= request.MinSize.Value);

            if (request.MaxSize.HasValue)
                query = query.Where(f => f.Size <= request.MaxSize.Value);

            if (request.FromCreateAt.HasValue)
                query = query.Where(f => f.CreatedAt >= request.FromCreateAt.Value);

            if (request.ToCreateAt.HasValue)
                query = query.Where(f => f.CreatedAt <= request.ToCreateAt.Value);

            if (request.FromUpdateAt.HasValue)
                query = query.Where(f => f.UpdatedAt >= request.FromUpdateAt.Value);

            if (request.ToUpdateAt.HasValue)
                query = query.Where(f => f.UpdatedAt <= request.ToUpdateAt.Value);

            return await query
                .AsNoTracking()
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => new Response(
                    f.Id.Value,
                    f.FileName.Value,
                    f.FileName.NameWithoutExtension,
                    f.FileName.Extension,
                    f.MimeType,
                    f.Size,
                    f.Status,
                    f.CreatedAt,
                    f.UpdatedAt))
                .ToPagedResultAsync(
                    request.Page,
                    request.PageSize,
                    cancellationToken);
        }
    }

    public class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v{v:apiVersion}/files", 
                    async ([AsParameters] GetAllFilesRequest request, ISender sender, CancellationToken cancellationToken) =>
                    {
                        var query = request.Adapt<Query>();
                        var result = await sender.Send(query, cancellationToken);
                        return result.ToHttpResult();
                    })
                .Version(app, 1.0)
                .WithName("GetAllFiles")
                .WithTags("Files");
        }
    }
}
