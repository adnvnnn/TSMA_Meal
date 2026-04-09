namespace Food_Reservation.Setup;

public static class AuthorizationRegister
{
    public static void RegisterAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole("Admin"));
        });
    }
}