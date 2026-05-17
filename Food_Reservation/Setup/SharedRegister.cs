using TSMA_Meal.Shared;
using TSMA_Meal.Shared.Repositories.OffDays;
using TSMA_Meal.Shared.Repositories.Otp;
using TSMA_Meal.Shared.Repositories.Personnel;
using TSMA_Meal.Shared.Services.Otp;

namespace TSMA_Meal.Setup;

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