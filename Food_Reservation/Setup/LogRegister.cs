namespace TSMA_Meal.Setup;

public static class LogRegister
{
    public static void RegisterLog(this ConfigurationManager configuration,string environmentName)
    {
        configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
    }
}