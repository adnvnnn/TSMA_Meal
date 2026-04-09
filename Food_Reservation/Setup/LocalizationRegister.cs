using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace Food_Reservation.Setup;

public static class LocalizationRegister
{
    public static void RegisterLocalization(this IServiceCollection services)
    {
        services.AddLocalization(options => options.ResourcesPath = "Resources");

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture("fa");
            options.SupportedCultures = new List<CultureInfo>
            {
                new("fa")
            };
            options.SupportedUICultures = new List<CultureInfo>
            {
                new("fa")
            };
            options.RequestCultureProviders = new List<IRequestCultureProvider>
            {
                new QueryStringRequestCultureProvider(),
                new CookieRequestCultureProvider
                {
                    CookieName = CookieRequestCultureProvider.DefaultCookieName
                }
            };
        });

    }
}