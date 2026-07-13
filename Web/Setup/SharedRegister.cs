using TSMA.Meal.Web.Shared.Repositories.OffDays;
using TSMA.Meal.Web.Shared.Repositories.Otp;
using TSMA.Meal.Web.Shared.Repositories.Personnel;
using TSMA.Meal.Web.Shared.Services.Otp;

namespace TSMA.Meal.Web.Setup;

public static class SharedRegister
{
    public static void RegisterShared(this IServiceCollection services)
    {
        services.AddScoped<IPersonnelRepository, PersonnelRepository>();

        services.AddScoped<IOffDaysRepository, OffDaysRepository>();

        services.AddScoped<IOtpRepository, OtpRepository>();

        services.AddScoped<IOtpService, OtpService>();

    }
}