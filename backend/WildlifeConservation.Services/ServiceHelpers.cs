namespace WildlifeConservation.Services;

internal static class ServiceHelpers
{
    public static DateTime AsUtc(DateTime value) => InputNormalization.AsUtc(value);

    public static DateTime? AsUtc(DateTime? value)
    {
        return InputNormalization.AsUtc(value);
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
