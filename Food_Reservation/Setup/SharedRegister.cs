using TSMA_Meal.Shared;

namespace TSMA_Meal.Setup;

public static class SharedRegister
{
    public static void RegisterShared(this IServiceCollection services)
    {
        services.AddScoped<IPersonnelRepository, PersonnelRepository>();

        services.AddScoped<IOffDaysRepository, OffDaysRepository>();

        services.AddScoped<IOtpRepository, OtpRepository>();
    }
}