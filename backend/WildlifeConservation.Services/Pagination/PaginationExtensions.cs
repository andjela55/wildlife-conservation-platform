namespace WildlifeConservation.Services.Pagination;

public static class PaginationExtensions
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        PaginationQuery pagination,
        CancellationToken cancellationToken = default)
    {
        var (pageNumber, pageSize) = Normalize(pagination);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>(items, pageNumber, pageSize, totalCount);
    }

    public static PagedResult<T> ToPagedResult<T>(
        this IReadOnlyList<T> items,
        PaginationQuery pagination)
    {
        var (pageNumber, pageSize) = Normalize(pagination);
        var totalCount = items.Count;
        var pageItems = items
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<T>(pageItems, pageNumber, pageSize, totalCount);
    }

    private static (int PageNumber, int PageSize) Normalize(PaginationQuery pagination)
    {
        var pageNumber = Math.Max(1, pagination.PageNumber);
        var pageSize = Math.Clamp(pagination.PageSize, 1, 100);

        return (pageNumber, pageSize);
    }
}
