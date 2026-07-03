namespace WildlifeConservation.Services;

internal static class ServiceHelpers
{
    public static DateTime AsUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }

    public static DateTime? AsUtc(DateTime? value)
    {
        return value.HasValue ? AsUtc(value.Value) : null;
    }

    public static string RequiredText(string value, string fieldName)
    {
        var trimmed = value.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            throw new ServiceException((int)HttpStatusCode.BadRequest, $"{fieldName} is required.");
        }

        return trimmed;
    }

    public static async Task<TEntity> EnsureFoundAsync<TEntity>(
        Task<TEntity?> entityTask,
        int id,
        string entityName)
        where TEntity : class
    {
        return await entityTask
            ?? throw new ServiceException((int)HttpStatusCode.NotFound, $"{entityName} with id {id} was not found.");
    }
}
