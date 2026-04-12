namespace TSMA_Meal.Setup;

public static class AuthorizationRegister
{
    public static void RegisterAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization();
    }
}