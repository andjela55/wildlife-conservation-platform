using System.Text.Json;
using WildlifeConservation.Shared;

namespace WildlifeConservation.Api.Middleware;

public class ServiceExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ServiceException ex)
        {
            context.Response.StatusCode = ex.StatusCode;
            context.Response.ContentType = "application/problem+json";

            var problem = new
            {
                type = "https://tools.ietf.org/html/rfc9110#section-15",
                title = ex.StatusCode switch
                {
                    StatusCodes.Status401Unauthorized => "Unauthorized",
                    StatusCodes.Status403Forbidden => "Forbidden",
                    StatusCodes.Status404NotFound => "Not Found",
                    _ => "Bad Request"
                },
                status = ex.StatusCode,
                detail = ex.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}
