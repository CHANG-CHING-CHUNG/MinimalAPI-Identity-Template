namespace MinimalApiIdentityTemplate.Extensions;

public class ContentTypeValidatorMiddleware
{
    private readonly RequestDelegate _next;

    public ContentTypeValidatorMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (httpContext.Request.Method != HttpMethods.Get && httpContext.Request.Method != HttpMethods.Delete &&
            httpContext.Request.ContentType != "application/json")
        {
            await httpContext.Response.WriteAsJsonAsync(new
            {
                Status = "Failure",
                Errors = new List<object>
                    { new { Code = "InvalidContentType", Description = "Content-Type must be application/json" } }
            });
            return;
        }

        await _next(httpContext);
    }
}

public static class ContentTypeValidatoreExtensions
{
    public static IApplicationBuilder UseContentTypeValidator(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ContentTypeValidatorMiddleware>();
    }
}