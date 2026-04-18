using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace TSMA_Meal.Features.Account;


public static class LogoutEndpoints
{
    public static void MapLogout(this IEndpointRouteBuilder app)
    {
        app.MapPost("/logout", async (HttpContext httpContext) =>
            {
                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                return Results.LocalRedirect("/login");
            })
            .WithTags("Authentication")
            .DisableAntiforgery();
    }
}