using Microsoft.Extensions.Logging;

namespace TSMA.Meal.Web.Features.Account.SendOtp.Sms;

/// <summary>
/// Demo stand-in for the IPPanel SMS service: instead of calling the provider,
/// it prints the OTP to the console so the app can be used without an SMS account.
/// </summary>
internal class ConsoleSmsService(ILogger<ConsoleSmsService> logger) : ISmsService
{
    public Task SendOtpAsync(string phone, string code)
    {
        Console.WriteLine();
        Console.WriteLine("==================================================");
        Console.WriteLine($"  DEMO OTP for {phone}: {code}");
        Console.WriteLine("==================================================");
        Console.WriteLine();

        logger.LogWarning("DEMO OTP for {Phone}: {Code}", phone, code);

        return Task.CompletedTask;
    }
}
