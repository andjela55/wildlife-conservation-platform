namespace WildlifeConservation.Api.Extensions;

public static class PagedResultMappingExtensions
{
    public static PagedResult<TDestination> MapPagedResult<TSource, TDestination>(
        this IMapper mapper,
        PagedResult<TSource> source)
    {
        return new PagedResult<TDestination>(
            mapper.Map<List<TDestination>>(source.Items),
            source.PageNumber,
            source.PageSize,
            source.TotalCount);
    }
}
