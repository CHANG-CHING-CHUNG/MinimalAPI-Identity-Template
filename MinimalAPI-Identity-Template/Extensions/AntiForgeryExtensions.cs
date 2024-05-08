using Microsoft.AspNetCore.Antiforgery;

namespace MinimalApiIdentityTemplate.Extensions;

public static class AntiForgeryExtensions
{
    public static TBuilder ValidateAntiforgery<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            try
            {
                var antiForgeryService = context.HttpContext.RequestServices.GetRequiredService<IAntiforgery>();
                await antiForgeryService.ValidateRequestAsync(context.HttpContext);
            }
            catch (AntiforgeryValidationException)
            {
                return Results.BadRequest(new
                {
                    Status = "Failure",
                    Errors = new List<object>
                    {
                        new { Code = "InvalidCsrfToken", Description = "Antiforgery token validation failed" }
                    }
                });
            }

            return await next(context);
        });
    }
}