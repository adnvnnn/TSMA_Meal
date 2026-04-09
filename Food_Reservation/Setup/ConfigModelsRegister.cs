using Food_Reservation.ConfigModels;

namespace Food_Reservation.Setup;

public static class ConfigModelsRegister
{
    public static void RegisterConfigModels(this ConfigurationManager configuration)
    {
        configuration.GetSection("ConnectionStrings").Get<ConnectionStringConfigModel>();
    }
}