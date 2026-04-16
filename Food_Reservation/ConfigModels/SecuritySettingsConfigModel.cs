namespace TSMA_Meal.ConfigModels;

public class SecuritySettingsConfigModel
{
    public static string LoginPath { get; set; } = null!;
    public static string LogoutPath { get; set; } = null!;
    public static int LoginDays { get; set; }
    public static int BanDurationHours { get; set; }
    public static int MaxOtpAttemptsPerDay { get; set; }
    public static int MaxCheckOtpAttemptsPerSend { get; set; }
    public static int OtpExpiryMinutes { get; set; }
}