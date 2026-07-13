using Microsoft.AspNetCore.Authentication.Cookies;
using TSMA.Meal.Web.ConfigModels;

namespace TSMA.Meal.Web.Setup;

public static class AuthenticationRegister
{
    public static void RegisterAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = SecuritySettingsConfigModel.LoginPath;
                options.LogoutPath = SecuritySettingsConfigModel.LogoutPath;
                options.ExpireTimeSpan = TimeSpan.FromDays(SecuritySettingsConfigModel.LoginDays);
                options.SlidingExpiration = true;
                options.Events = new CookieAuthenticationEvents
                {

                    OnRedirectToLogin = context =>
                    {
                        context.Response.Redirect(SecuritySettingsConfigModel.LoginPath);
                        return Task.CompletedTask;
                    }
                };
            });
    }
}