namespace TSMA_Meal.Features.Meals.MealsList;

public static class MealsListRegister
{
    public static void RegisterMealsList(this IServiceCollection services)
    {
        services.AddScoped<IMealsListRepository, MealsListRepository>();
    }
}