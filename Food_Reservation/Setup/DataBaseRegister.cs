using Food_Reservation.ConfigModels;
using Food_Reservation.Database;
using Microsoft.EntityFrameworkCore;

namespace Food_Reservation.Setup;

public static class DataBaseRegister
{
    public static void RegisterDataBase(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(ConnectionStringConfigModel.Default);
        });
    }
}