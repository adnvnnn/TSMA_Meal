using Food_Reservation.Shared;

namespace Food_Reservation.Setup;

public static class SharedRegister
{
    public static void RegisterShared(this IServiceCollection services)
    {
        services.AddScoped<IPersonnelRepository, PersonnelRepository>();

        services.AddScoped<IOffDaysRepository, OffDaysRepository>();
    }
}