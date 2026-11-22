using Microsoft.EntityFrameworkCore;
using TSMA.Meal.Web.ConfigModels;
using TSMA.Meal.Web.Database;

namespace TSMA.Meal.Web.Setup;

public static class DataBaseRegister
{
    public static void RegisterDataBase(this IServiceCollection services, bool demoMode = false)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            if (demoMode)
            {
                options.UseInMemoryDatabase("TsmaMealDemo");

                return;
            }

            options.UseSqlServer(ConnectionStringConfigModel.Default);
        });

    }
}
