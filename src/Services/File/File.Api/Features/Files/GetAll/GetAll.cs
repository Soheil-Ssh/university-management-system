namespace File.Api.Features.Files.GetAll;

public static class GetAll
{
    public sealed record GetAllFilesRequest(int Page = 1, int PageSize = 20);

    public sealed record Response(Guid Id,
        string FileName,
        string MimeType,
        long Size,
        FileStatus Status,
        DateTime CreateAt,
        DateTime UpdateAt);

    public sealed record Query(int Page = 1, int PageSize = 20) : IQuery<Result<PagedResult<Response>>>;

    public class Handler(FileDbContext context) : IQueryHandler<Query, Result<PagedResult<Response>>>
    {
        public async Task<Result<PagedResult<Response>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = context.Files.AsQueryable();

            // TODO Add filters

            return await query.AsNoTracking()
                .Select(f => new Response(
                    f.Id.Value,
                    f.FileName.NameWithoutExtension,
                    f.MimeType,
                    f.Size,
                    f.Status,
                    f.CreatedAt,
                    f.UpdatedAt))
                .ToPagedResultAsync(request.Page, request.PageSize, cancellationToken);
        }
    }

    public class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v{v:apiVersion}/files", async ([AsParameters] GetAllFilesRequest request, ISender sender) =>
            {
                var query = new Query(request.Page, request.PageSize);
                var result = await sender.Send(query);
                return result.ToHttpResult();
            })
            .Version(app, 1.0)
            .WithName("GetAllFiles")
            .WithTags("Files");
        }
    }
}
