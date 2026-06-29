namespace SharedKernel.Api.Contracts;

public class PagedResult<T>
{
    public PagedResult() { }

    public PagedResult(
        IReadOnlyList<T> items,
        int currentPage,
        int totalCount,
        int pageSize)
    {
        Items = items;
        CurrentPage = currentPage;
        TotalCount = totalCount;
        PageSize = pageSize;
        TotalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
    }

    public IReadOnlyList<T> Items { get; init; } = [];
    public int CurrentPage { get; init; }
    public int TotalPages { get; init; }
    public int TotalCount { get; init; }
    public int PageSize { get; init; }
    public bool IsExistNextPage => CurrentPage < TotalPages;
    public bool IsExistPrevPage => CurrentPage > 1;
}