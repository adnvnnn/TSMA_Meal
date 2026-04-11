using TSMA_Meal.ConfigModels;

namespace TSMA_Meal.Setup;

public static class ConfigModelsRegister
{
    public static void RegisterConfigModels(this ConfigurationManager configuration)
    {
        configuration.GetSection("ConnectionStrings").Get<ConnectionStringConfigModel>();

        configuration.GetSection("SmsService").Get<SmsConfigModel>();

    }
}