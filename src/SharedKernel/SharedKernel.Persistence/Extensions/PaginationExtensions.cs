using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions.Pagination;

namespace SharedKernel.Persistence.Extensions;

public static class PaginationExtensions
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(this IQueryable<T> query, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Max(1, pageSize);

        var totalCount = await query.CountAsync(cancellationToken);

        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));

        page = Math.Min(page, totalPages);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>(items, page, totalCount, pageSize);
    }
}