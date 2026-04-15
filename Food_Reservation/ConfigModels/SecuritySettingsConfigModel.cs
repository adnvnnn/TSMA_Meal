namespace TSMA_Meal.ConfigModels;

public class SecuritySettingsConfigModel
{
    public static int LoginDays { get; set; }
    public static int BanDurationHours { get; set; }
    public static int MaxOtpAttemptsPerDay { get; set; }
    public static int MaxCheckOtpAttemptsPerSend { get; set; }
    public static int OtpExpiryMinutes { get; set; }
}