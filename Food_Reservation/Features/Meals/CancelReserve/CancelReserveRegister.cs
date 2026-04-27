namespace TSMA_Meal.Features.Meals.CancelReserve;

public static class CancelReserveRegister
{
    public static void RegisterCancelReserve(this IServiceCollection services)
    {

        services.AddScoped<ICancelReserveRepository, CancelReserveRepository>();
    }
}