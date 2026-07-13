using Microsoft.EntityFrameworkCore;
using TSMA.Meal.Web.ConfigModels;
using TSMA.Meal.Web.Database;

namespace TSMA.Meal.Web.Setup;

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