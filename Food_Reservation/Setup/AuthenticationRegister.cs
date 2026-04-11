using Microsoft.AspNetCore.Authentication.Cookies;

namespace TSMA_Meal.Setup;

public static class AuthenticationRegister
{
    public static void RegisterAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
                options.ExpireTimeSpan = TimeSpan.FromDays(60);
                options.SlidingExpiration = true;

                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = context =>
                    {
                        context.Response.Redirect("/login");
                        return Task.CompletedTask;
                    }
                };
            });
    }
}