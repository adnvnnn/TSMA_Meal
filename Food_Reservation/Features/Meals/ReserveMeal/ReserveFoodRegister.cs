namespace TSMA_Meal.Features.Meals.ReserveMeal;


public static class ReserveFoodRegister
{
    public static void RegisterReserveFood(this IServiceCollection services)
    {
        services.AddScoped<IReserveFoodRepository, ReserveFoodRepository>();
    }
}
