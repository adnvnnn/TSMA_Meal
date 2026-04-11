namespace TSMA_Meal.ConfigModels;

public class SmsConfigModel
{
    public static string Username { get; set; } = null!;
    public static string Domain { get; set; } = null!;
    public static string Password { get; set; } = null!;
    public static string Sender { get; set; } = null!;
    public static string Url { get; set; } = null!;

    public static string VerificationPatternCode { get; set; } = null!;

    public const string ParameterName = "verification-code";

}