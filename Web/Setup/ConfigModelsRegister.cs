using TSMA.Meal.Web.ConfigModels;

namespace TSMA.Meal.Web.Setup;

public static class ConfigModelsRegister
{
    public static void RegisterConfigModels(this ConfigurationManager configuration)
    {
        configuration.GetSection("ConnectionStrings").Get<ConnectionStringConfigModel>();

        configuration.GetSection("SmsService").Get<SmsConfigModel>();

        configuration.GetSection("SecuritySettings").Get<SecuritySettingsConfigModel>();

        configuration.GetSection("ApiConfig").Get<BaseUrlConfigModel>();

    }
}