using Microsoft.EntityFrameworkCore;
using TSMA_Meal.ConfigModels;
using TSMA_Meal.Database;

namespace TSMA_Meal.Setup;

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